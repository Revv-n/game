using System;
using GreenT.AssetBundles;
using GreenT.Settings;
using UnityEngine;

namespace StripClub.Model.Data;

public abstract class DataLoader<T> : ILoader<T> where T : UnityEngine.Object
{
	protected readonly IAssetBundlesLoader assetBundlesLoader;

	protected readonly ProjectSettings projectSettings;

	public string Path { get; private set; }

	public bool IsAbsolutePath { get; private set; }

	public DataLoader(IAssetBundlesLoader assetBundlesLoader, ProjectSettings projectSettings, string path, bool isAbsolutePath = true)
	{
		this.assetBundlesLoader = assetBundlesLoader;
		this.projectSettings = projectSettings;
		Path = path;
		IsAbsolutePath = isAbsolutePath;
	}

	public abstract IObservable<T> Load();

	public virtual void SetPath(string path, bool isAbsolutePath = true)
	{
		Path = path;
		IsAbsolutePath = isAbsolutePath;
	}
}
