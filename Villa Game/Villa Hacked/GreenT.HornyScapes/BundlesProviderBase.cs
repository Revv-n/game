using System;
using System.Collections.Generic;
using GreenT.AssetBundles;
using StripClub.Model.Shop.Data;
using UnityEngine;

namespace GreenT.HornyScapes;

public class BundlesProviderBase
{
	protected Dictionary<ContentSource, List<IAssetBundle>> _contentBundles;

	public BundlesProviderBase()
	{
		_contentBundles = new Dictionary<ContentSource, List<IAssetBundle>>();
	}

	public void TryAdd(ContentSource contentSource, IAssetBundle assetBundle)
	{
		if (!_contentBundles.ContainsKey(contentSource))
		{
			_contentBundles[contentSource] = new List<IAssetBundle>();
		}
		if (!_contentBundles[contentSource].Contains(assetBundle))
		{
			_contentBundles[contentSource].Add(assetBundle);
		}
	}

	public T TryFindInConcreteBundle<T>(ContentSource contentSource, string bundleName, bool silent = false) where T : UnityEngine.Object
	{
		List<IAssetBundle> bundles = TryGetBundles(contentSource);
		T val = null;
		try
		{
			val = TryLoadBundle<T>(bundles, bundleName);
			if (val == null && contentSource != 0)
			{
				val = TryFindInConcreteBundle<T>(ContentSource.Default, bundleName);
			}
		}
		catch (Exception)
		{
		}
		return val;
	}

	private T TryLoadBundle<T>(List<IAssetBundle> bundles, string bundleName) where T : UnityEngine.Object
	{
		for (int i = 0; i < bundles.Count; i++)
		{
			IAssetBundle assetBundle = bundles[i];
			if (assetBundle.Contains(bundleName))
			{
				return assetBundle.LoadAsset<T>(bundleName);
			}
		}
		return null;
	}

	private List<IAssetBundle> TryGetBundles(ContentSource contentSource)
	{
		if (_contentBundles.TryGetValue(contentSource, out var value))
		{
			return value;
		}
		return null;
	}
}
