using System;
using System.Text.RegularExpressions;

namespace GreenT.Utilities;

public class GetArgumentFromUrlUtility
{
	public string TryGetArgumentFromUrl(string url, string argumentName)
	{
		try
		{
			string query = new Uri(url).Query;
			return ParseQueryParameter(query, argumentName);
		}
		catch (Exception)
		{
			return null;
		}
	}

	private string ParseQueryParameter(string query, string parameter)
	{
		Match match = new Regex(parameter + "=([^&]*)").Match(query);
		if (!match.Success)
		{
			return null;
		}
		return match.Groups[1].Value;
	}
}
