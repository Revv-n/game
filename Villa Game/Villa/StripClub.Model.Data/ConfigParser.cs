using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using StripClub.Utility;
using UnityEngine;

namespace StripClub.Model.Data;

public class ConfigParser
{
	public class Folder
	{
		public string Name { get; private set; }

		public List<Folder> Folders { get; private set; } = new List<Folder>();


		public List<File> Files { get; set; } = new List<File>();


		public Folder(string name)
		{
			Name = name;
		}

		public Folder FindFolder(string name)
		{
			if (Name.Equals(name))
			{
				return this;
			}
			return Folders.FirstOrDefault((Folder _folder) => _folder.FindFolder(name) != null);
		}

		public File FindFileByName(string name)
		{
			File file = Files.FirstOrDefault((File _file) => _file.name.Equals(name));
			if (file != null)
			{
				return file;
			}
			IEnumerator<Folder> enumerator = Folders.GetEnumerator();
			while (file == null && enumerator.MoveNext())
			{
				file = enumerator.Current.FindFileByName(name);
			}
			return file;
		}

		public File FindFile(Func<File, bool> func)
		{
			File file = Files.FirstOrDefault(func);
			if (file != null)
			{
				return file;
			}
			IEnumerator<Folder> enumerator = Folders.GetEnumerator();
			while (file == null && enumerator.MoveNext())
			{
				file = enumerator.Current.FindFile(func);
			}
			return file;
		}
	}

	public class File
	{
		public string name;

		public string comment;

		public string hash;

		public string content;

		public string url;
	}

	public static Folder GetConfigsFolderForVersion(string version, string json)
	{
		return GetConfigFolder(json).Folders.FirstOrDefault((Folder f) => f.Name == version);
	}

	public static Folder GetConfigFolder(string json)
	{
		Folder rootFolder = new Folder("root");
		ConstructFolders(ref rootFolder, json);
		return rootFolder;
	}

	public static void ConstructFolders(ref Folder rootFolder, string json)
	{
		bool flag = false;
		foreach (DictionaryEntry item in JsonConvert.DeserializeObject<Hashtable>(json))
		{
			if (item.Key is string)
			{
				Folder folder = new Folder(item.Key.ToString());
				Hashtable hashtable = JsonConvert.DeserializeObject<Hashtable>(item.Value.ToString());
				if (hashtable["files"] != null)
				{
					folder.Files = hashtable["files"].ToString().AsArray<File>().ToList();
					rootFolder.Folders.Add(folder);
				}
				else
				{
					flag = true;
					Debug.LogError(string.Format("{0}: Folder '{1}' doesnt have files", "ConfigParser", item.Key));
				}
			}
			else
			{
				flag = true;
				Debug.LogError(string.Format("{0}:Folder Name is not type of string: {1}", "ConfigParser", item.Key));
			}
		}
		if (flag)
		{
			Debug.LogError("ConfigParser:" + json);
		}
	}
}
