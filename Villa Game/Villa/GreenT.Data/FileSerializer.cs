using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using GreenT.HornyScapes.Saves;
using UniRx;
using UnityEngine;

namespace GreenT.Data;

public class FileSerializer : ISaveProvider, ISaveSerializer, ISaveLoader
{
	private readonly User user;

	public FileSerializer(User user)
	{
		this.user = user;
	}

	public void Serialize(SavedData state)
	{
		if (!string.IsNullOrWhiteSpace(user.PlayerID))
		{
			using (FileStream stream = new FileStream(Application.persistentDataPath + "/" + user.PlayerID, FileMode.OpenOrCreate))
			{
				SerializeStream(state, stream);
				return;
			}
		}
		Debug.LogError("Incorrect file name");
	}

	private void SerializeStream(object data, Stream stream)
	{
		IFormatter formatter = new BinaryFormatter();
		try
		{
			formatter.Serialize(stream, data);
		}
		catch (Exception innerException)
		{
			throw innerException.SendException("Exception on serialize data");
		}
		finally
		{
			stream.Close();
		}
	}

	IObservable<SavedData> ISaveLoader.LoadSave()
	{
		return Observable.Start((Func<SavedData>)DeserializeFile, Scheduler.MainThreadIgnoreTimeScale);
	}

	private SavedData DeserializeFile()
	{
		SavedData savedData = null;
		if (string.IsNullOrWhiteSpace(user.PlayerID))
		{
			throw new ArgumentException("Incorrect file name");
		}
		string path = Application.persistentDataPath + "/" + user.PlayerID;
		if (!File.Exists(path))
		{
			return null;
		}
		using (FileStream stream = new FileStream(path, FileMode.Open))
		{
			try
			{
				savedData = DeserializeStream<SavedData>(stream);
			}
			catch (Exception)
			{
			}
		}
		if (savedData == null)
		{
			using FileStream stream2 = new FileStream(path, FileMode.Open);
			try
			{
				List<Memento> data = DeserializeStream<List<Memento>>(stream2);
				savedData = new SavedData(0L, data);
				savedData.ClearDummies();
			}
			catch (Exception)
			{
			}
		}
		return savedData;
	}

	private T DeserializeStream<T>(Stream stream)
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
