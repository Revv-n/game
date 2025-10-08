using System;
using System.Collections.Generic;
using System.Linq;

namespace Games.Coresdk.Unity;

public class TokenCollection
{
	private Dictionary<string, string> dict;

	public static TokenCollection Parse(string url)
	{
		if (string.IsNullOrEmpty(url))
		{
			return new TokenCollection(new Dictionary<string, string>());
		}
		return Parse(new Uri(url));
	}

	private static TokenCollection Parse(Uri uri)
	{
		return new TokenCollection((from o in uri.Query.Substring(uri.Query.IndexOf('?') + 1).Split('&', StringSplitOptions.None)
			select o.Split('=', StringSplitOptions.None) into items
			where items.Count() == 2
			select items).ToDictionary((string[] pair) => Uri.UnescapeDataString(pair[0]), (string[] pair) => Uri.UnescapeDataString(pair[1])));
	}

	private TokenCollection(Dictionary<string, string> dict)
	{
		this.dict = dict;
	}

	public string GetValue(string key, string defaultValue = "")
	{
		string value = null;
		if (dict.TryGetValue(key, out value))
		{
			return value;
		}
		return defaultValue;
	}
}
