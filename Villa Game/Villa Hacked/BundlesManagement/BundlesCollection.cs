using System.Collections.Generic;
using System.Linq;
using GreenT.AssetBundles;
using UnityEngine;

namespace BundlesManagement;

internal class BundlesCollection
{
	private Dictionary<string, IAssetBundle> m_Bundles;

	public string[] names => m_Bundles.Keys.ToArray();

	public IAssetBundle[] bundles => m_Bundles.Values.ToArray();

	internal BundlesCollection()
	{
		m_Bundles = new Dictionary<string, IAssetBundle>();
	}

	public void AddBundlesInCollection(params IAssetBundle[] bundles)
	{
		foreach (IAssetBundle bundle in bundles)
		{
			TryAddBundle(bundle);
		}
	}

	private bool TryAddBundle(IAssetBundle bundle)
	{
		if (bundle == null)
		{
			return false;
		}
		string key = GetKey(bundle);
		if (Contains(key))
		{
			return false;
		}
		m_Bundles[key] = bundle;
		Debug.Log("Add bundle '" + key + "'");
		return true;
	}

	public void RemoveBundlesFromCollection(params IAssetBundle[] bundles)
	{
		RemoveBundlesFromCollection(bundles.Select((IAssetBundle b) => GetKey(b)).ToArray());
	}

	public void RemoveBundlesFromCollection(params string[] bundles)
	{
		foreach (string bundle in bundles)
		{
			TryRemoveBundle(bundle);
		}
	}

	private bool TryRemoveBundle(string bundle)
	{
		if (!Contains(bundle))
		{
			return false;
		}
		m_Bundles.Remove(bundle);
		Debug.Log("Remove bundle " + bundle);
		return true;
	}

	public bool TryGet(string bundleName, out IAssetBundle bundle)
	{
		return (bundle = Get(bundleName)) != null;
	}

	public IAssetBundle Get(string bundleName)
	{
		if (!Contains(bundleName))
		{
			Debug.LogError("Can't get bundle '" + bundleName + "'");
			return null;
		}
		return m_Bundles[bundleName];
	}

	public bool Contains(IAssetBundle bundle)
	{
		return Contains(GetKey(bundle));
	}

	public bool Contains(string bundle)
	{
		return m_Bundles.ContainsKey(bundle ?? "");
	}

	private string GetKey(IAssetBundle bundle)
	{
		return bundle?.name ?? "";
	}
}
