using System;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

namespace StripClub.Utility;

public static class JsonHelper
{
	[Serializable]
	private class Wrapper<T>
	{
		public T[] array;
	}

	public static T[] AsArray<T>(this string json)
	{
		StringBuilder stringBuilder = WrapJson(json);
		try
		{
			return JsonConvert.DeserializeObject<Wrapper<T>>(stringBuilder.ToString()).array;
		}
		catch (Exception ex)
		{
			Debug.LogError(ex);
			throw ex;
		}
	}

	public static StringBuilder WrapJson(string json)
	{
		bool num = json[0].Equals('[');
		StringBuilder stringBuilder = new StringBuilder("{ \"array\": ");
		if (!num)
		{
			stringBuilder.Append('[');
		}
		stringBuilder.Append(json);
		if (!num)
		{
			stringBuilder.Append(']');
		}
		stringBuilder.Append('}');
		return stringBuilder;
	}
}
