using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using CompressString;
using UnityEngine;

public static class BinarySerializer
{
	private class BinderFixer : SerializationBinder
	{
		private Dictionary<string, string> assamblyException = new Dictionary<string, string>();

		private Dictionary<string, string> typeException = new Dictionary<string, string>();

		public BinderFixer(Dictionary<string, string> assamblyException, Dictionary<string, string> typeException)
		{
			this.assamblyException = assamblyException;
			this.typeException = typeException;
		}

		public override Type BindToType(string assemblyName, string typeName)
		{
			if (fixedAssamblyException.Any() && fixedAssamblyException.TryGetValue(assemblyName, out var value))
			{
				Debug.Log("BinaryDeserialize fix: Fix Assamply name from [" + assemblyName + "] to [" + value + "]");
				assemblyName = value;
			}
			if (fixedTypeException.Any() && fixedTypeException.TryGetValue(typeName, out var value2))
			{
				Debug.Log("BinaryDeserialize fix: Fix Type name from [" + typeName + "] to [" + value2 + "]");
				typeName = value2;
			}
			Type type = null;
			try
			{
				type = Type.GetType($"{typeName}, {assemblyName}");
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
				Debug.LogError($"Type for typeName [{type}] and assamblyName [{assemblyName}] wasn't found!");
			}
			return type;
		}
	}

	private static Dictionary<string, string> fixedAssamblyException = new Dictionary<string, string>();

	private static Dictionary<string, string> fixedTypeException = new Dictionary<string, string> { { "Plagins.DLAnalyticsParameter", "Plugins.DLAnalyticsParameter" } };

	public static T LoadPreferences<T>(string pref_name) where T : new()
	{
		T val = default(T);
		try
		{
			val = BinaryDeserialize<T>(pref_name);
		}
		catch (Exception exception)
		{
			Debug.LogErrorFormat("Can't load preferences with name [{0}]!", pref_name);
			Debug.LogException(exception);
		}
		if (EqualityComparer<T>.Default.Equals(val))
		{
			val = new T();
		}
		return val;
	}

	public static void SavePreferences<T>(string name, T obj)
	{
		BinarySerialize(GetFullPath(name), obj);
	}

	public static void OnlyJsonWrite(string filename, string data)
	{
		string finaly_data = data.ToString(CultureInfo.InvariantCulture);
		JsonWriteTask(GetFullPath(filename), filename, finaly_data);
	}

	public static void JsonWrite(string filename, string data)
	{
		string finaly_data = data.ToString(CultureInfo.InvariantCulture);
		JsonWriteTask(GetFullPath(filename), filename, finaly_data);
		JsonServerWrite(filename, finaly_data);
	}

	private static void JsonWriteTask(string persitstent_filename, string filename, string finaly_data)
	{
		if (string.IsNullOrEmpty(finaly_data))
		{
			return;
		}
		try
		{
			if (!File.Exists(persitstent_filename + ".json"))
			{
				File.WriteAllText(persitstent_filename + ".json", StringCompressor.CompressString(finaly_data));
			}
			File.WriteAllText(persitstent_filename + "_new.json", StringCompressor.CompressString(finaly_data));
		}
		catch (Exception ex)
		{
			Debug.LogError("Ex: " + ex);
		}
		finally
		{
			if (!string.IsNullOrEmpty(finaly_data))
			{
				File.Replace(persitstent_filename + "_new.json", persitstent_filename + ".json", persitstent_filename + ".bac", ignoreMetadataErrors: true);
			}
		}
	}

	private static void JsonServerWrite(string filename, string finaly_data)
	{
		if (!string.IsNullOrEmpty(finaly_data))
		{
			PlayerPrefs.SetString(filename + ".json", finaly_data);
			PlayerPrefs.Save();
		}
	}

	public static JsonData JsonRead(string name)
	{
		StringCompressor.DecompressString(File.ReadAllText(GetFullPath(name)), out var result);
		return new JsonData(result.ToString(CultureInfo.InvariantCulture));
	}

	public static void BinarySerialize(string filename, object data)
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		using FileStream fileStream = File.Open(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite);
		try
		{
			binaryFormatter.Serialize(fileStream, data);
		}
		catch (SerializationException ex)
		{
			Debug.LogErrorFormat("BinarySerialize failed for [{0}] with message [{1}]", fileStream, ex.Message);
			fileStream.Close();
		}
		finally
		{
			fileStream.Close();
		}
	}

	public static T DeepClone<T>(T obj)
	{
		using MemoryStream memoryStream = new MemoryStream();
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		binaryFormatter.Serialize(memoryStream, obj);
		memoryStream.Position = 0L;
		return (T)binaryFormatter.Deserialize(memoryStream);
	}

	public static object DeepClone(object obj)
	{
		using MemoryStream memoryStream = new MemoryStream();
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		binaryFormatter.Serialize(memoryStream, obj);
		memoryStream.Position = 0L;
		return binaryFormatter.Deserialize(memoryStream);
	}

	public static T BinaryDeserialize<T>(string filename) where T : new()
	{
		T result = new T();
		if (!FileExists(filename))
		{
			return result;
		}
		using (FileStream fileStream = File.Open(GetFullPath(filename), FileMode.Open, FileAccess.Read))
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			binaryFormatter.Binder = new BinderFixer(fixedAssamblyException, fixedTypeException);
			try
			{
				result = (T)binaryFormatter.Deserialize(fileStream);
			}
			catch (SerializationException ex)
			{
				Debug.Log("BinaryDeserialize failed: " + ex.Message);
				fileStream.Close();
			}
			finally
			{
				fileStream.Close();
			}
		}
		return result;
	}

	public static string SerializeToString(object data)
	{
		using MemoryStream memoryStream = new MemoryStream();
		new BinaryFormatter().Serialize(memoryStream, data);
		memoryStream.Flush();
		memoryStream.Position = 0L;
		return Convert.ToBase64String(memoryStream.ToArray());
	}

	public static bool DeserializeFromString<T>(string server_data, out T result) where T : new()
	{
		byte[] buffer = Convert.FromBase64String(server_data);
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		result = new T();
		using (MemoryStream memoryStream = new MemoryStream(buffer))
		{
			try
			{
				memoryStream.Seek(0L, SeekOrigin.Begin);
				object obj = binaryFormatter.Deserialize(memoryStream);
				if (obj is T)
				{
					result = (T)obj;
					return true;
				}
				Debug.LogErrorFormat("Can't deserialize server data of type [{0}]!", typeof(T));
			}
			catch (SerializationException ex)
			{
				Debug.LogError("BinaryDeserialize failed: " + ex.Message);
				return false;
			}
		}
		return false;
	}

	public static bool FileExists(string filename)
	{
		return File.Exists(GetFullPath(filename));
	}

	public static void Delete(string filename)
	{
		File.Delete(GetFullPath(filename));
	}

	public static string GetFullPath(string filename)
	{
		string text = "";
		text = "/";
		return Application.persistentDataPath + text + filename;
	}

	public static string GetFullPath(string filename, string file_extention)
	{
		string text = "";
		text = "/";
		return $"{Application.persistentDataPath}{text}{filename}.{file_extention}";
	}
}
