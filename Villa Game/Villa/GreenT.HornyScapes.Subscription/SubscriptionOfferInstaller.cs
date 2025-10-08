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
		base.Container.BindInterfacesAndSelfTo<SubscriptionRewardService>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<SubscriptionOfferPushService>().AsSingle().WithArguments(_previewWindowOpener, _offerSectionController, _subscriptionOfferWindowID, _offerWindowID);
	}

	private void BindBank()
	{
		base.Container.Bind<SubscriptionOfferWindow>().FromInstance(_offerWindow).AsSingle();
		base.Container.BindInterfacesTo<SubscriptionOfferSectionFactory>().FromInstance(_offerSectionFactory).AsCached();
		base.Container.Bind<AbstractSectionManager<LayoutType, SubscriptionPushSettings, SubscriptionOfferSectionView>>().To<SubscriptionOfferSectionManager>().FromInstance(_offerSectionManager)
			.AsSingle();
		base.Container.Bind<SubscriptionOfferSectionController>().FromInstance(_offerSectionController).AsSingle();
		base.Container.BindSignal<LotBoughtSignal>().ToMethod((SubscriptionOfferWindow x) => x.OnLotBought).FromResolve();
	}
}
