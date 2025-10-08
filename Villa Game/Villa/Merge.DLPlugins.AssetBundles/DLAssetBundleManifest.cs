using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace Merge.DLPlugins.AssetBundles;

[Serializable]
public class DLAssetBundleManifest : Dictionary<string, List<DLAssetBundleObject>>
{
	private List<DLAssetBundleObject> allObjects = new List<DLAssetBundleObject>();

	public List<DLAssetBundleObject> AllObjects => allObjects;

	public Hash128 GetAssetBundleHash(string bundle_name)
	{
		Hash128 result = default(Hash128);
		string cached_name = bundle_name;
		if (cached_name.Contains('/'))
		{
			cached_name = cached_name.Split('/').Last();
		}
		DLAssetBundleObject dLAssetBundleObject = allObjects.FirstOrDefault((DLAssetBundleObject x) => x.Name == cached_name);
		if ((bool)dLAssetBundleObject)
		{
			return dLAssetBundleObject.Hash;
		}
		return result;
	}

	public bool CacheAssetExist(string bundle_name)
	{
		if (bundle_name.Contains('/'))
		{
			bundle_name = bundle_name.Split('/').Last();
		}
		if ((bool)allObjects.FirstOrDefault((DLAssetBundleObject x) => x.Name == bundle_name))
		{
			return true;
		}
		return false;
	}

	public string GetBundleUrl(string bundle_name)
	{
		string cached_name = bundle_name;
		if (cached_name.Contains('/'))
		{
			cached_name = cached_name.Split('/').Last();
		}
		DLAssetBundleObject dLAssetBundleObject = allObjects.FirstOrDefault((DLAssetBundleObject x) => x.Name == cached_name);
		if (!dLAssetBundleObject)
		{
			return bundle_name;
		}
		return dLAssetBundleObject.Url;
	}

	public static implicit operator bool(DLAssetBundleManifest manifest)
	{
		return manifest != null;
	}

	public static bool operator ==(DLAssetBundleManifest p1, DLAssetBundleManifest p2)
	{
		return object.Equals(p1, p2);
	}

	public static bool operator !=(DLAssetBundleManifest p1, DLAssetBundleManifest p2)
	{
		return !object.Equals(p1, p2);
	}

	public override bool Equals(object obj)
	{
		if (obj is DLAssetBundleManifest other)
		{
			return Equals(other);
		}
		return false;
	}

	public bool Equals(DLAssetBundleManifest other)
	{
		if (!other)
		{
			return false;
		}
		if (allObjects.IsNullOrEmpty() && other.IsNullOrEmpty())
		{
			return true;
		}
		if (allObjects.Count != other.allObjects.Count)
		{
			return false;
		}
		for (int i = 0; i < allObjects.Count; i++)
		{
			if (!allObjects[i])
			{
				if ((bool)other.allObjects[i])
				{
					return false;
				}
			}
			else if (!allObjects[i].Equals(other.allObjects[i]))
			{
				return false;
			}
		}
		return true;
	}

	public static DLAssetBundleManifest Read()
	{
		string path = Path.Combine(BinarySerializer.GetFullPath("dlassetbundlemanifest.json"));
		if (!File.Exists(path))
		{
			return new DLAssetBundleManifest();
		}
		try
		{
			DLAssetBundleManifest dLAssetBundleManifest = JsonConvert.DeserializeObject<DLAssetBundleManifest>(File.ReadAllText(path));
			foreach (KeyValuePair<string, List<DLAssetBundleObject>> item in dLAssetBundleManifest)
			{
				dLAssetBundleManifest.AllObjects.AddRange(item.Value);
			}
			return dLAssetBundleManifest;
		}
		catch (Exception exception)
		{
			Debug.LogError("HASH MANIFEST LOG: Trouble with parsing json from file, return clean manifest!");
			Debug.LogException(exception);
			return new DLAssetBundleManifest();
		}
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}
}
