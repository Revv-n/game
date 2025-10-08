using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.AssetBundles;
using GreenT.Settings;
using GreenT.Settings.Data;
using UniRx;
using UniRx.Diagnostics;
using UnityEngine;

namespace StripClub.Model.Data;

public class ShopDataManager<T> : ILoader<IEnumerable<T>>, IDataStorage<T> where T : ShopObject
{
	private List<T> contentBase;

	private IAssetBundlesLoader bundleLoader;

	private IProjectSettings projectSettings;

	public ShopDataManager(IAssetBundlesLoader loader, IProjectSettings projectSettings)
	{
		contentBase = new List<T>();
		bundleLoader = loader;
		this.projectSettings = projectSettings;
	}

	public void Add(T content)
	{
		contentBase.Add(content);
	}

	public void AddRange(IEnumerable<T> content)
	{
		contentBase.AddRange(content);
	}

	public IObservable<IEnumerable<T>> Load()
	{
		return ObservableDebugExtensions.Debug<IEnumerable<T>>(Observable.Do<IEnumerable<T>>(Observable.CatchIgnore<IEnumerable<T>, Exception>(bundleLoader.GetAssets<T>(projectSettings.BundleUrlResolver.BundleUrl(BundleType.Shop)), (Action<Exception>)delegate(Exception ex)
		{
			Debug.LogError(ex);
		}), (Action<IEnumerable<T>>)Init), "Load Store Assortement");
	}

	public void Init(IEnumerable<T> initCollection)
	{
		contentBase.Clear();
		contentBase.AddRange(initCollection);
	}

	public IEnumerable<K> GetData<K>() where K : T
	{
		return contentBase.OfType<K>();
	}
}
