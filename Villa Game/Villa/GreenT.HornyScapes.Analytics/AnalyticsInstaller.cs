using GreenT.Data;
using GreenT.HornyScapes.Analytics.Starshops;
using GreenT.HornyScapes.Analytics.Windows;
using GreenT.Net;
using reenT.HornyScapes.Analytics;
using StripClub.Model.Shop.Data;
using Zenject;

namespace GreenT.HornyScapes.Analytics;

public class AnalyticsInstaller : Installer<AnalyticsInstaller>
{
	public override void InstallBindings()
	{
		BindSingle<AnalyticSystemManager>();
		BindSingle<AnalyticStarter>();
		Installer<AnalyticSendersInstaller>.Install(base.Container);
		BindUserStats();
		BindSubSystems();
		BindSystems();
		BindPlatformInstaller();
	}

	private void BindPlatformInstaller()
	{
		Installer<PlatformAnalyticInstaller>.Install(base.Container);
	}

	private void BindUserStats()
	{
		base.Container.BindInterfacesAndSelfTo<UserStats>().AsSingle().OnInstantiated<UserStats>(AddToSaver)
			.NonLazy();
		base.Container.BindSignal<LotBoughtSignal>().ToMethod((UserStats x) => x.UpdatePaymentStats).FromResolve();
	}

	private void BindSubSystems()
	{
		base.Container.BindInterfacesAndSelfTo<LinkedContentAnalyticDataFactory>().AsSingle();
		BindSingle<DeviceSystem>();
		BindSingle<ResponseStatusSystem>();
		base.Container.BindInterfacesAndSelfTo<CurrencyAnalyticProcessingService>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<CurrencyAnalyticFactory>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<CurrencyAmplitudeAnalytic>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<EnergyAnalyticHolder>().AsSingle();
	}

	private void BindSystems()
	{
		Bind<InstallAnalytic>();
		Bind<BankAnalytic>();
		base.Container.BindSignal<LotBoughtSignal>().ToMethod(delegate(BankAnalytic x, LotBoughtSignal y)
		{
			x.SendEventIfIsValid(y.Lot);
		}).FromResolve();
		Bind<GirlAnalityc>();
		BindStarShopAnalytic();
		Bind<StoryAnalytic>();
		Bind<EventFirstTimePushAnalytic>();
		Bind<EventFirstTimeStartAnalytic>();
		Bind<EventRewardAnalytic>();
		Bind<MiniEventFirstTimeSeenAnalytic>();
		Bind<MiniEventTaskAnalytic>();
		Bind<RouletteAnalytic>();
		base.Container.BindSignal<RouletteLotBoughtSignal>().ToMethod(delegate(RouletteAnalytic x, RouletteLotBoughtSignal y)
		{
			x.SendEventIfIsValid(y.Lot);
		}).FromResolve();
		Bind<EventEnergySpendAnalytic>();
		Bind<EnergySpendAnalytic>();
		Bind<BattlePassFirstTimePushAnalitic>();
		Bind<BattlePassFirstTimeStartAnalytic>();
		Bind<BattlePassFreeRewardAnalytic>();
		Bind<BattlePassPremiumRewardAnalytic>();
		Bind<EventTaskAnalytic>();
		Bind<OfferPushAnalytic>();
		Bind<OfferClickAnalytic>();
	}

	private void BindStarShopAnalytic()
	{
		base.Container.BindInterfacesAndSelfTo<StarShopAnalytic>().AsSingle().OnInstantiated(delegate(InjectContext context, StarShopAnalytic obj)
		{
			base.Container.Resolve<AnalyticSystemManager>().Add(obj);
			AddToSaver(context, obj);
		})
			.NonLazy();
	}

	public static void AddToSaver(InjectContext context, ISavableState savable)
	{
		context.Container.Resolve<ISaver>().Add(savable);
	}

	private void BindSingle<T>()
	{
		base.Container.BindInterfacesAndSelfTo<T>().AsSingle();
	}

	private void Bind<T>() where T : BaseAnalytic
	{
		base.Container.BindInterfacesAndSelfTo<T>().AsSingle().OnInstantiated(delegate(InjectContext context, T obj)
		{
			base.Container.Resolve<AnalyticSystemManager>().Add(obj);
		})
			.NonLazy();
	}
}
