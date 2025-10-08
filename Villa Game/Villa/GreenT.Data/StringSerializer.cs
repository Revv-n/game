using System;
using System.IO;
using GreenT.HornyScapes.Saves;

namespace GreenT.Data;

public class StringSerializer
{
	public T Deserialize<T>(string dataString) where T : new()
	{
		using MemoryStream stream = new MemoryStream(Convert.FromBase64String(dataString));
		return GameDataSerializator.DeserializeStream<T>(stream);
	}

	public string Serialize(SavedData state)
	{
		string empty = string.Empty;
		using MemoryStream memoryStream = new MemoryStream();
		GameDataSerializator.SerializeStream(state.Data, memoryStream);
		return Convert.ToBase64String(memoryStream.ToArray());
	}
}
