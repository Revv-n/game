using System;
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
		Installer<AnalyticSendersInstaller>.Install(((InstallerBase)this).Container);
		BindUserStats();
		BindSubSystems();
		BindSystems();
		BindPlatformInstaller();
	}

	private void BindPlatformInstaller()
	{
		Installer<PlatformAnalyticInstaller>.Install(((InstallerBase)this).Container);
	}

	private void BindUserStats()
	{
		((NonLazyBinder)((InstantiateCallbackConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<UserStats>()).AsSingle()).OnInstantiated<UserStats>((Action<InjectContext, UserStats>)AddToSaver)).NonLazy();
		((BindSignalToBinder<LotBoughtSignal>)(object)SignalExtensions.BindSignal<LotBoughtSignal>(((InstallerBase)this).Container)).ToMethod<UserStats>((Func<UserStats, Action<LotBoughtSignal>>)((UserStats x) => x.UpdatePaymentStats)).FromResolve();
	}

	private void BindSubSystems()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<LinkedContentAnalyticDataFactory>()).AsSingle();
		BindSingle<DeviceSystem>();
		BindSingle<ResponseStatusSystem>();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<CurrencyAnalyticProcessingService>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<CurrencyAnalyticFactory>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<CurrencyAmplitudeAnalytic>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<EnergyAnalyticHolder>()).AsSingle();
	}

	private void BindSystems()
	{
		Bind<InstallAnalytic>();
		Bind<BankAnalytic>();
		((BindSignalToBinder<LotBoughtSignal>)(object)SignalExtensions.BindSignal<LotBoughtSignal>(((InstallerBase)this).Container)).ToMethod<BankAnalytic>((Action<BankAnalytic, LotBoughtSignal>)delegate(BankAnalytic x, LotBoughtSignal y)
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
		((BindSignalToBinder<RouletteLotBoughtSignal>)(object)SignalExtensions.BindSignal<RouletteLotBoughtSignal>(((InstallerBase)this).Container)).ToMethod<RouletteAnalytic>((Action<RouletteAnalytic, RouletteLotBoughtSignal>)delegate(RouletteAnalytic x, RouletteLotBoughtSignal y)
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
		((NonLazyBinder)((InstantiateCallbackConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<StarShopAnalytic>()).AsSingle()).OnInstantiated<StarShopAnalytic>((Action<InjectContext, StarShopAnalytic>)delegate(InjectContext context, StarShopAnalytic obj)
		{
			((InstallerBase)this).Container.Resolve<AnalyticSystemManager>().Add(obj);
			AddToSaver(context, obj);
		})).NonLazy();
	}

	public static void AddToSaver(InjectContext context, ISavableState savable)
	{
		context.Container.Resolve<ISaver>().Add(savable);
	}

	private void BindSingle<T>()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<T>()).AsSingle();
	}

	private void Bind<T>() where T : BaseAnalytic
	{
		((NonLazyBinder)((InstantiateCallbackConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<T>()).AsSingle()).OnInstantiated<T>((Action<InjectContext, T>)delegate(InjectContext context, T obj)
		{
			((InstallerBase)this).Container.Resolve<AnalyticSystemManager>().Add(obj);
		})).NonLazy();
	}
}
