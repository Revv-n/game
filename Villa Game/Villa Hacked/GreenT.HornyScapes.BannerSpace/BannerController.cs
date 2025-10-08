using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Saves;
using UniRx;

namespace GreenT.HornyScapes.BannerSpace;

public class BannerController : IDisposable
{
	private readonly BannerNotificationService _bannerNotificationService;

	private readonly BannerCluster _bannerCluster;

	private readonly BannerFactory _bannerFactory;

	private readonly PreloadContentService _preloadContentService;

	private readonly CompositeDisposable _disposables = new CompositeDisposable();

	public IBannerNotificationService BannerNotificationService => _bannerNotificationService;

	public BannerController(BannerNotificationService bannerNotificationService, BannerCluster bannerCluster, BannerFactory bannerFactory, PreloadContentService preloadContentService)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		_bannerNotificationService = bannerNotificationService;
		_bannerCluster = bannerCluster;
		_bannerFactory = bannerFactory;
		_preloadContentService = preloadContentService;
	}

	public void Initialization()
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Banner>(Observable.SelectMany<Banner, Banner>(Observable.Select<CreateData, Banner>(_bannerNotificationService.OnBannerLoaded, (Func<CreateData, Banner>)_bannerFactory.Create), (Func<Banner, IObservable<Banner>>)LoadDependencies), (Action<Banner>)OnBannerCreated), (ICollection<IDisposable>)_disposables);
	}

	private IObservable<Banner> LoadDependencies(Banner banner)
	{
		return Observable.Select<Unit, Banner>(_preloadContentService.GetPreloadRewardsStream(banner.GetAllRewards()), (Func<Unit, Banner>)((Unit _) => banner));
	}

	private void OnBannerCreated(Banner banner)
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<bool>(Observable.Take<bool>(Observable.Where<bool>((IObservable<bool>)banner.Locker.IsOpen, (Func<bool, bool>)((bool x) => !x)), 1), (Action<bool>)delegate
		{
			CloseBanner(banner);
		}), (ICollection<IDisposable>)_disposables);
		_bannerCluster.AddBanner(banner);
		_bannerNotificationService.NotifyBannerReady(banner.Id);
	}

	private void CloseBanner(Banner banner)
	{
		_bannerCluster.RemoveBanner(banner);
		_bannerNotificationService.NotifyBannerClose(banner.Id);
	}

	public bool HaveBanner(int id)
	{
		return _bannerCluster.HaveReadyBanner(id);
	}

	public void Dispose()
	{
		CompositeDisposable disposables = _disposables;
		if (disposables != null)
		{
			disposables.Clear();
		}
	}
}
