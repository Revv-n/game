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
		base.Container.Bind<OfferWindow>().FromInstance(offerWindow).AsSingle();
		base.Container.BindInterfacesTo<OfferSectionFactory>().FromInstance(sectionFactory).AsCached();
		base.Container.Bind<AbstractSectionManager<LayoutType, OfferSettings, OfferSectionView>>().To<SectionManager>().FromInstance(sectionManager)
			.AsSingle();
		base.Container.Bind<SectionController>().FromInstance(sectionController).AsSingle();
		base.Container.BindSignal<LotBoughtSignal>().ToMethod((OfferWindow x) => x.OnLotBought).FromResolve();
	}
}
