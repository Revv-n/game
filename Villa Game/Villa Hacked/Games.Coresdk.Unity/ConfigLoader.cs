using System;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Games.Coresdk.Unity;

public class ConfigLoader
{
	private class EnptyedJsonFile
	{
		private string filepath;

		private DESCryptoServiceProvider des;

		public bool IsExists => File.Exists(filepath);

		public EnptyedJsonFile(string filepath)
		{
			this.filepath = filepath;
			des = new DESCryptoServiceProvider();
			des.Key = Encoding.ASCII.GetBytes("TM^jRt@0");
			des.IV = Encoding.ASCII.GetBytes("q6prn^Jb");
		}

		public string Read()
		{
			byte[] array = File.ReadAllBytes(filepath);
			using MemoryStream memoryStream = new MemoryStream();
			try
			{
				using CryptoStream cryptoStream = new CryptoStream(memoryStream, des.CreateDecryptor(), CryptoStreamMode.Write);
				cryptoStream.Write(array, 0, array.Length);
				cryptoStream.FlushFinalBlock();
				return Encoding.UTF8.GetString(memoryStream.ToArray());
			}
			catch (Exception)
			{
				File.Delete(filepath);
				return string.Empty;
			}
		}

		public void Write(string contents)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(contents);
			using MemoryStream memoryStream = new MemoryStream();
			using CryptoStream cryptoStream = new CryptoStream(memoryStream, des.CreateEncryptor(), CryptoStreamMode.Write);
			cryptoStream.Write(bytes, 0, bytes.Length);
			cryptoStream.FlushFinalBlock();
			File.WriteAllBytes(filepath, memoryStream.ToArray());
		}
	}

	private const string DEFAULT_FILENAME = "coresdk_config";

	public static string JSON_FILENAME = "coresdk_config";

	private EnptyedJsonFile jsonFile;

	private JSONNode domainConfig;

	public static string PREF_KEY_DOMAIN => "Games.Coresdk.Unity." + JSON_FILENAME;

	public Config config { get; private set; }

	public ConfigLoader()
	{
		jsonFile = new EnptyedJsonFile(Application.persistentDataPath + "/" + JSON_FILENAME);
	}

	public IEnumerator CoInit()
	{
		yield return CoUpdateConfig();
		yield return CoFindLoginDomain();
	}

	private IEnumerator CoUpdateConfig()
	{
		JSONNode jSONNode = ReadConfig();
		if (jSONNode == null)
		{
			yield break;
		}
		JSONNode.Enumerator enumerator = jSONNode["config"].AsArray.GetEnumerator();
		while (enumerator.MoveNext())
		{
			yield return CoDownloadConfig(enumerator.Current.Value, delegate(string jsonText)
			{
				if (!string.IsNullOrEmpty(jsonText))
				{
					domainConfig = JSON.Parse(jsonText);
					SaveConfig(jsonText);
				}
			});
			if (domainConfig != null)
			{
				break;
			}
		}
	}

	private IEnumerator CoFindLoginDomain()
	{
		if (domainConfig == null)
		{
			yield break;
		}
		string @string = PlayerPrefs.GetString(PREF_KEY_DOMAIN, "");
		if (!string.IsNullOrEmpty(@string))
		{
			yield return CoCheckLoginDomain(@string);
			if (config != null)
			{
				yield break;
			}
		}
		JSONNode.Enumerator enumerator = domainConfig["login"].AsArray.GetEnumerator();
		while (enumerator.MoveNext())
		{
			yield return CoCheckLoginDomain(enumerator.Current.Value);
			if (config != null)
			{
				break;
			}
		}
	}

	private IEnumerator CoCheckLoginDomain(string domain)
	{
		UnityWebRequest www = UnityWebRequest.Get(domain + "/api/checkLoginUrl");
		try
		{
			yield return www.SendWebRequest();
			if (!www.isNetworkError && !www.isHttpError)
			{
				config = new Config(domain, domainConfig["payment"].Value);
				PlayerPrefs.SetString(PREF_KEY_DOMAIN, domain);
				yield break;
			}
		}
		finally
		{
			((IDisposable)www)?.Dispose();
		}
	}

	private IEnumerator CoDownloadConfig(string url, Action<string> callback)
	{
		UnityWebRequest www = UnityWebRequest.Get(url);
		try
		{
			yield return www.SendWebRequest();
			if (!www.isNetworkError && !www.isHttpError)
			{
				callback(www.downloadHandler.text);
				yield break;
			}
			callback(string.Empty);
		}
		finally
		{
			((IDisposable)www)?.Dispose();
		}
	}

	private JSONNode ReadConfig()
	{
		string text = string.Empty;
		if (jsonFile.IsExists)
		{
			text = jsonFile.Read();
		}
		if (string.IsNullOrEmpty(text))
		{
			text = Resources.Load<TextAsset>(JSON_FILENAME).text;
		}
		return JSON.Parse(text);
	}

	private void SaveConfig(string jsonText)
	{
		jsonFile.Write(jsonText);
	}
}
