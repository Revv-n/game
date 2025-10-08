using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace MISC;

public static class Serializer
{
	public static void Save<T>(T data, string path)
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate);
		try
		{
			binaryFormatter.Serialize(fileStream, data);
		}
		catch (Exception)
		{
			Debug.LogError($"{data.GetType()} incorrect format");
			throw;
		}
		fileStream.Close();
	}

	public static T Load<T>(string path)
	{
		if (!File.Exists(path))
		{
			return default(T);
		}
		try
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			FileStream fileStream = new FileStream(path, FileMode.Open);
			T result = (T)binaryFormatter.Deserialize(fileStream);
			fileStream.Close();
			return result;
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			return default(T);
		}
	}

	public static void Remove(string path)
	{
		if (File.Exists(path))
		{
			File.Delete(path);
			Debug.Log("Deleted: " + path);
		}
	}
}
