using System;
using UniRx;

namespace GreenT.HornyScapes.BannerSpace;

public class BannerNotificationService : IBannerNotificationService
{
	private readonly Subject<CreateData> _bannerLoadedSubject = new Subject<CreateData>();

	private readonly Subject<int> _bannerReadySubject = new Subject<int>();

	private readonly Subject<int> _bannerCloseSubject = new Subject<int>();

	public IObservable<CreateData> OnBannerLoaded => _bannerLoadedSubject;

	public IObservable<int> OnBannerReady => _bannerReadySubject;

	public IObservable<int> OnBannerClose => _bannerCloseSubject;

	public void NotifyBannerLoaded(CreateData createData)
	{
		_bannerLoadedSubject.OnNext(createData);
	}

	public void NotifyBannerReady(int id)
	{
		_bannerReadySubject.OnNext(id);
	}

	public void NotifyBannerClose(int id)
	{
		_bannerCloseSubject.OnNext(id);
	}
}
