using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace GreenT.Data;

public static class GameDataSerializator
{
	public static void Serialize(object mapper, string fileName = null)
	{
		if (!string.IsNullOrWhiteSpace(fileName))
		{
			using (FileStream stream = new FileStream(Application.persistentDataPath + "/" + fileName, FileMode.OpenOrCreate))
			{
				SerializeStream(mapper, stream);
				return;
			}
		}
		Debug.LogError("Incorrect file name");
	}

	public static void SerializeStream(object mapper, Stream stream)
	{
		IFormatter formatter = new BinaryFormatter();
		try
		{
			formatter.Serialize(stream, mapper);
		}
		catch (Exception innerException)
		{
			throw innerException.SendException("Ошибка при попытке сериализации объекта ");
		}
		finally
		{
			stream.Close();
		}
	}

	public static T Deserialize<T>(string fileName = null)
	{
		if (string.IsNullOrWhiteSpace(fileName))
		{
			throw new ArgumentException("Incorrect file name");
		}
		string text = Application.persistentDataPath + "/" + fileName;
		if (!File.Exists(text))
		{
			return default(T);
		}
		using FileStream stream = new FileStream(text, FileMode.Open);
		try
		{
			return DeserializeStream<T>(stream);
		}
		catch (Exception innerException)
		{
			throw innerException.SendException("Не удалось десериализовать данные по пути: " + text);
		}
	}

	public static T DeserializeStream<T>(Stream stream)
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter
		{
			Binder = new MigrationSerializationBinder()
		};
		try
		{
			return (T)binaryFormatter.Deserialize(stream);
		}
		catch (Exception ex)
		{
			throw ex.SendException(MigrationSerializationBinder.GetErrorMessage(ex.Message));
		}
		finally
		{
			stream.Close();
		}
	}
}
