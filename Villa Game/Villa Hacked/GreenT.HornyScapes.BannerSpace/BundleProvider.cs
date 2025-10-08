using System;
using System.Collections.Generic;
using UniRx;

namespace GreenT.HornyScapes.BannerSpace;

public class BundleProvider
{
	private readonly BackgroundBundleLoader _backgroundBundleLoader;

	private readonly Dictionary<string, BannerBackgroundBundle> _bannerBundles = new Dictionary<string, BannerBackgroundBundle>();

	public BundleProvider(BackgroundBundleLoader backgroundBundleLoader)
	{
		_backgroundBundleLoader = backgroundBundleLoader;
	}

	public IObservable<CreateData> Get(CreateData createData)
	{
		string backgroundName = createData.Info.BackgroundName;
		if (!TryGetBannerFromCache(backgroundName, out var bannerBackgroundBundle))
		{
			return LoadAndCacheBanner(backgroundName, createData);
		}
		return ReturnCachedBanner(createData, bannerBackgroundBundle);
	}

	private bool TryGetBannerFromCache(string backgroundName, out BannerBackgroundBundle bannerBackgroundBundle)
	{
		return _bannerBundles.TryGetValue(backgroundName, out bannerBackgroundBundle);
	}

	private IObservable<CreateData> ReturnCachedBanner(CreateData createData, BannerBackgroundBundle bannerBackgroundBundle)
	{
		createData.SetBundle(bannerBackgroundBundle);
		return Observable.Return<CreateData>(createData);
	}

	private IObservable<CreateData> LoadAndCacheBanner(string backgroundName, CreateData createData)
	{
		return Observable.AsObservable<CreateData>(Observable.Select<BannerBackgroundBundle, CreateData>(Observable.Do<BannerBackgroundBundle>(Observable.Do<BannerBackgroundBundle>(_backgroundBundleLoader.Load(backgroundName), (Action<BannerBackgroundBundle>)delegate(BannerBackgroundBundle bundle)
		{
			CacheBanner(backgroundName, bundle);
		}), (Action<BannerBackgroundBundle>)createData.SetBundle), (Func<BannerBackgroundBundle, CreateData>)((BannerBackgroundBundle _) => createData)));
	}

	private void CacheBanner(string backgroundName, BannerBackgroundBundle backgroundBundle)
	{
		_bannerBundles.TryAdd(backgroundName, backgroundBundle);
	}
}
