using System;
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
		_bannerNotificationService = bannerNotificationService;
		_bannerCluster = bannerCluster;
		_bannerFactory = bannerFactory;
		_preloadContentService = preloadContentService;
	}

	public void Initialization()
	{
		_bannerNotificationService.OnBannerLoaded.Select(_bannerFactory.Create).SelectMany((Func<Banner, IObservable<Banner>>)LoadDependencies).Subscribe(OnBannerCreated)
			.AddTo(_disposables);
	}

	private IObservable<Banner> LoadDependencies(Banner banner)
	{
		return from _ in _preloadContentService.GetPreloadRewardsStream(banner.GetAllRewards())
			select banner;
	}

	private void OnBannerCreated(Banner banner)
	{
		banner.Locker.IsOpen.Where((bool x) => !x).Take(1).Subscribe(delegate
		{
			CloseBanner(banner);
		})
			.AddTo(_disposables);
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
		_disposables?.Clear();
	}
}
