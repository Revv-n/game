using System;
using System.Text;
using GreenT.Settings.Data;
using Newtonsoft.Json;
using StripClub.Model.Data;
using StripClub.Utility;

namespace GreenT.HornyScapes.Data;

public static class ContentParser
{
	private const string DefaultEmptyContent = "[]";

	public static string GetContentString(this ConfigParser.Folder folder, RequestType configType, string configVersion = null)
	{
		try
		{
			if (!string.IsNullOrEmpty(configVersion))
			{
				folder = folder.FindFolder(configVersion);
			}
			if (folder == null)
			{
				new ArgumentNullException("Folder doesn't contain config version: " + configVersion).LogException();
				return "[]";
			}
			RequestType result;
			ConfigParser.File file = folder.FindFile((ConfigParser.File _file) => Enum.TryParse<RequestType>(_file.name, out result) && result.Equals(configType));
			if (file == null)
			{
				new ArgumentNullException("Folder doesn't contain file: " + configType).LogException();
				return "[]";
			}
			return file.content;
		}
		catch (Exception innerException)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append($"Can't find file of type {configType}");
			if (!configVersion.Equals(null))
			{
				stringBuilder.Append(" of version \"" + configVersion + "\"");
			}
			stringBuilder.Append(" in configuration files hierarchy");
			throw innerException.SendException(stringBuilder.ToString());
		}
	}

	public static T[] GetContentArray<T>(this ConfigParser.Folder folder, RequestType configType, string configVersion = null)
	{
		try
		{
			return folder.GetContentString(configType, configVersion).AsArray<T>();
		}
		catch (Exception innerException)
		{
			throw innerException.SendException("Can't extract data of type " + typeof(T).ToString() + "  from the configuration file \"" + configType.ToString() + "\" of version \"" + configVersion + "\"");
		}
	}

	public static T GetContent<T>(this ConfigParser.Folder folder, RequestType configType, string configVersion = null)
	{
		try
		{
			return JsonConvert.DeserializeObject<T>(folder.GetContentString(configType, configVersion));
		}
		catch (Exception innerException)
		{
			throw innerException.SendException("Can't extract data of type " + typeof(T).ToString() + "  from the configuration file \"" + configType.ToString() + "\" of version \"" + configVersion + "\"");
		}
	}
}
