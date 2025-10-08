using System;

namespace GreenT.HornyScapes.BannerSpace;

public interface IBannerNotificationService
{
	IObservable<CreateData> OnBannerLoaded { get; }

	IObservable<int> OnBannerReady { get; }

	IObservable<int> OnBannerClose { get; }
}
