using System;
using GreenT.HornyScapes.Bank.Data;
using GreenT.HornyScapes.Bank.Offer.UI;
using GreenT.HornyScapes.Bank.UI;
using GreenT.HornyScapes.Subscription.Push;
using GreenT.UI;
using StripClub.Model.Shop.Data;
using StripClub.UI.Shop;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Subscription;

public class SubscriptionOfferInstaller : MonoInstaller
{
	[SerializeField]
	private WindowID _subscriptionOfferWindowID;

	[SerializeField]
	private WindowID _offerWindowID;

	[SerializeField]
	private WindowOpener _previewWindowOpener;

	[SerializeField]
	private SubscriptionOfferWindow _offerWindow;

	[SerializeField]
	private SubscriptionOfferSectionFactory _offerSectionFactory;

	[SerializeField]
	private SubscriptionOfferSectionManager _offerSectionManager;

	[SerializeField]
	private SubscriptionOfferSectionController _offerSectionController;

	public override void InstallBindings()
	{
		BindPush();
		BindBank();
	}

	private void BindPush()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<SubscriptionRewardService>()).AsSingle();
		((ArgConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<SubscriptionOfferPushService>()).AsSingle()).WithArguments<WindowOpener, SubscriptionOfferSectionController, WindowID, WindowID>(_previewWindowOpener, _offerSectionController, _subscriptionOfferWindowID, _offerWindowID);
	}

	private void BindBank()
	{
		((FromBinderGeneric<SubscriptionOfferWindow>)(object)((MonoInstallerBase)this).Container.Bind<SubscriptionOfferWindow>()).FromInstance(_offerWindow).AsSingle();
		((MonoInstallerBase)this).Container.BindInterfacesTo<SubscriptionOfferSectionFactory>().FromInstance((object)_offerSectionFactory).AsCached();
		((ConcreteBinderGeneric<AbstractSectionManager<LayoutType, SubscriptionPushSettings, SubscriptionOfferSectionView>>)(object)((MonoInstallerBase)this).Container.Bind<AbstractSectionManager<LayoutType, SubscriptionPushSettings, SubscriptionOfferSectionView>>()).To<SubscriptionOfferSectionManager>().FromInstance(_offerSectionManager).AsSingle();
		((FromBinderGeneric<SubscriptionOfferSectionController>)(object)((MonoInstallerBase)this).Container.Bind<SubscriptionOfferSectionController>()).FromInstance(_offerSectionController).AsSingle();
		((BindSignalToBinder<LotBoughtSignal>)(object)SignalExtensions.BindSignal<LotBoughtSignal>(((MonoInstallerBase)this).Container)).ToMethod<SubscriptionOfferWindow>((Func<SubscriptionOfferWindow, Action<LotBoughtSignal>>)((SubscriptionOfferWindow x) => x.OnLotBought)).FromResolve();
	}
}
