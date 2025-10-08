using System;
using System.Collections.Generic;
using GreenT.AssetBundles;
using GreenT.HornyScapes;
using StripClub.Model;
using StripClub.Model.Shop.Data;
using UniRx;

namespace GreenT.Settings.Data;

public class AssetBundleLoadProcessor : IDisposable
{
	private readonly Dictionary<string, ContentSource> _sourceMap = new Dictionary<string, ContentSource> { 
	{
		"Sellout",
		ContentSource.Sellout
	} };

	private readonly string _bundle;

	private readonly ILocker _locker;

	private readonly IAssetBundlesLoader _bundlesLoader;

	private readonly BundlesProviderBase _bundlesProvider;

	private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

	private readonly IProjectSettings _projectSettings;

	public AssetBundleLoadProcessor(string bundle, ILocker locker, IAssetBundlesLoader bundlesLoader, IProjectSettings projectSettings, BundlesProviderBase bundlesProvider)
	{
		_bundle = bundle;
		_locker = locker;
		_bundlesLoader = bundlesLoader;
		_projectSettings = projectSettings;
		_bundlesProvider = bundlesProvider;
	}

	public void Initialize()
	{
		_locker.IsOpen.Where((bool x) => x).Take(1).Debug("Downloading bundle " + _bundle, LogType.BundleLoad)
			.ContinueWith((bool _) => _bundlesLoader.DownloadAssetBundle(_projectSettings.BundleUrlResolver.BundleUrl(_bundle)))
			.Debug("Bundle " + _bundle + " loaded successfully", LogType.BundleLoad)
			.Subscribe(SetupBundle)
			.AddTo(_compositeDisposable);
	}

	private void SetupBundle(IAssetBundle bundle)
	{
		ContentSource valueOrDefault = _sourceMap.GetValueOrDefault(_bundle, ContentSource.Default);
		_bundlesProvider.TryAdd(valueOrDefault, bundle);
	}

	public void Dispose()
	{
		_compositeDisposable?.Dispose();
	}
}
