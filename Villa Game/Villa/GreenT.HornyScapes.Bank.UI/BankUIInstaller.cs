using GreenT.HornyScapes.Bank.BankTabs;
using ModestTree;
using StripClub.UI.Rewards;
using StripClub.UI.Shop;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Bank.UI;

public class BankUIInstaller : MonoInstaller<BankUIInstaller>
{
	[SerializeField]
	private BankWindow bankWindow;

	[SerializeField]
	private BankTabView tabViewPrefab;

	[SerializeField]
	private Transform tabsContainer;

	[SerializeField]
	private SectionFactory sectionFactory;

	[SerializeField]
	private BankTabUIController tabController;

	[SerializeField]
	private SectionManager sectionManager;

	[SerializeField]
	private SectionController sectionController;

	[SerializeField]
	private RouletteSectionFactory rouletteSectionFactory;

	[SerializeField]
	private RouletteSectionManager rouletteSectionManager;

	[SerializeField]
	private RouletteSectionController rouletteSectionController;

	public override void InstallBindings()
	{
		Assert.IsNotNull(bankWindow);
		Assert.IsNotNull(tabViewPrefab);
		Assert.IsNotNull(tabsContainer);
		Assert.IsNotNull(sectionFactory);
		Assert.IsNotNull(tabController);
		Assert.IsNotNull(sectionManager);
		Assert.IsNotNull(sectionController);
		Assert.IsNotNull(rouletteSectionFactory);
		Assert.IsNotNull(rouletteSectionManager);
		Assert.IsNotNull(rouletteSectionController);
		base.Container.BindInterfacesAndSelfTo<BankWindow>().FromInstance(bankWindow).AsSingle();
		base.Container.BindIFactory<BankTabView>().FromComponentInNewPrefab(tabViewPrefab).UnderTransform(tabsContainer);
		base.Container.BindInterfacesTo<SectionFactory>().FromInstance(sectionFactory).AsCached();
		base.Container.BindInterfacesTo<RouletteSectionFactory>().FromInstance(rouletteSectionFactory).AsCached();
		base.Container.Bind<BankTabUIController>().FromInstance(tabController).AsSingle();
		base.Container.BindInterfacesTo<BankTabView.Manager>().FromNewComponentOn(tabsContainer.gameObject).AsSingle();
		base.Container.Bind<AbstractSectionManager<LayoutType, BankTab, BankSectionView>>().To<SectionManager>().FromInstance(sectionManager)
			.AsSingle();
		base.Container.Bind<AbstractSectionManager<LayoutType, BankTab, RouletteLotSectionView>>().To<RouletteSectionManager>().FromInstance(rouletteSectionManager)
			.AsSingle();
		base.Container.Bind<SectionController>().FromInstance(sectionController).AsSingle();
		base.Container.Bind<RouletteSectionController>().FromInstance(rouletteSectionController).AsSingle();
		base.Container.BindSignal<ViewUpdateSignal>().ToMethod((BankWindow x) => x.OnUpdateRequest).FromResolve();
		base.Container.BindSignal<OpenTabSignal>().ToMethod((BankTabUIController x) => x.SelectTabBySignal).FromResolve();
		base.Container.Bind<BankSectionRedirectDispatcher>().FromNew().AsSingle();
		base.Container.Bind<GirlPromoOpener>().AsSingle();
	}
}
