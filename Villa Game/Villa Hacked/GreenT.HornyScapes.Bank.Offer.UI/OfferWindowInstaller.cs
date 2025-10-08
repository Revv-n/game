using System;
using GreenT.HornyScapes.Bank.Data;
using GreenT.HornyScapes.Bank.UI;
using StripClub.Model.Shop.Data;
using StripClub.UI.Shop;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Bank.Offer.UI;

public class OfferWindowInstaller : MonoInstaller
{
	[SerializeField]
	private OfferWindow offerWindow;

	[SerializeField]
	private OfferSectionFactory sectionFactory;

	[SerializeField]
	private SectionManager sectionManager;

	[SerializeField]
	private SectionController sectionController;

	public override void InstallBindings()
	{
		((FromBinderGeneric<OfferWindow>)(object)((MonoInstallerBase)this).Container.Bind<OfferWindow>()).FromInstance(offerWindow).AsSingle();
		((MonoInstallerBase)this).Container.BindInterfacesTo<OfferSectionFactory>().FromInstance((object)sectionFactory).AsCached();
		((ConcreteBinderGeneric<AbstractSectionManager<LayoutType, OfferSettings, OfferSectionView>>)(object)((MonoInstallerBase)this).Container.Bind<AbstractSectionManager<LayoutType, OfferSettings, OfferSectionView>>()).To<SectionManager>().FromInstance(sectionManager).AsSingle();
		((FromBinderGeneric<SectionController>)(object)((MonoInstallerBase)this).Container.Bind<SectionController>()).FromInstance(sectionController).AsSingle();
		((BindSignalToBinder<LotBoughtSignal>)(object)SignalExtensions.BindSignal<LotBoughtSignal>(((MonoInstallerBase)this).Container)).ToMethod<OfferWindow>((Func<OfferWindow, Action<LotBoughtSignal>>)((OfferWindow x) => x.OnLotBought)).FromResolve();
	}
}
