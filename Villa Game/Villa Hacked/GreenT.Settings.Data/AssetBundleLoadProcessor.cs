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
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		_bundle = bundle;
		_locker = locker;
		_bundlesLoader = bundlesLoader;
		_projectSettings = projectSettings;
		_bundlesProvider = bundlesProvider;
	}

	public void Initialize()
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<IAssetBundle>(Observable.ContinueWith<bool, IAssetBundle>(Observable.Take<bool>(Observable.Where<bool>((IObservable<bool>)_locker.IsOpen, (Func<bool, bool>)((bool x) => x)), 1).Debug("Downloading bundle " + _bundle, LogType.BundleLoad), (Func<bool, IObservable<IAssetBundle>>)((bool _) => _bundlesLoader.DownloadAssetBundle(_projectSettings.BundleUrlResolver.BundleUrl(_bundle)))).Debug("Bundle " + _bundle + " loaded successfully", LogType.BundleLoad), (Action<IAssetBundle>)SetupBundle), (ICollection<IDisposable>)_compositeDisposable);
	}

	private void SetupBundle(IAssetBundle bundle)
	{
		ContentSource valueOrDefault = CollectionExtensions.GetValueOrDefault<string, ContentSource>((IReadOnlyDictionary<string, ContentSource>)_sourceMap, _bundle, ContentSource.Default);
		_bundlesProvider.TryAdd(valueOrDefault, bundle);
	}

	public void Dispose()
	{
		CompositeDisposable compositeDisposable = _compositeDisposable;
		if (compositeDisposable != null)
		{
			compositeDisposable.Dispose();
		}
	}
}
