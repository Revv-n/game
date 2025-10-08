using System;
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
		Assert.IsNotNull((object)bankWindow);
		Assert.IsNotNull((object)tabViewPrefab);
		Assert.IsNotNull((object)tabsContainer);
		Assert.IsNotNull((object)sectionFactory);
		Assert.IsNotNull((object)tabController);
		Assert.IsNotNull((object)sectionManager);
		Assert.IsNotNull((object)sectionController);
		Assert.IsNotNull((object)rouletteSectionFactory);
		Assert.IsNotNull((object)rouletteSectionManager);
		Assert.IsNotNull((object)rouletteSectionController);
		((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<BankWindow>().FromInstance((object)bankWindow).AsSingle();
		((TransformScopeConcreteIdArgConditionCopyNonLazyBinder)((FactoryFromBinderBase)((MonoInstallerBase)this).Container.BindIFactory<BankTabView>()).FromComponentInNewPrefab((UnityEngine.Object)tabViewPrefab)).UnderTransform(tabsContainer);
		((MonoInstallerBase)this).Container.BindInterfacesTo<SectionFactory>().FromInstance((object)sectionFactory).AsCached();
		((MonoInstallerBase)this).Container.BindInterfacesTo<RouletteSectionFactory>().FromInstance((object)rouletteSectionFactory).AsCached();
		((FromBinderGeneric<BankTabUIController>)(object)((MonoInstallerBase)this).Container.Bind<BankTabUIController>()).FromInstance(tabController).AsSingle();
		((FromBinder)((MonoInstallerBase)this).Container.BindInterfacesTo<BankTabView.Manager>()).FromNewComponentOn(tabsContainer.gameObject).AsSingle();
		((ConcreteBinderGeneric<AbstractSectionManager<LayoutType, BankTab, BankSectionView>>)(object)((MonoInstallerBase)this).Container.Bind<AbstractSectionManager<LayoutType, BankTab, BankSectionView>>()).To<SectionManager>().FromInstance(sectionManager).AsSingle();
		((ConcreteBinderGeneric<AbstractSectionManager<LayoutType, BankTab, RouletteLotSectionView>>)(object)((MonoInstallerBase)this).Container.Bind<AbstractSectionManager<LayoutType, BankTab, RouletteLotSectionView>>()).To<RouletteSectionManager>().FromInstance(rouletteSectionManager).AsSingle();
		((FromBinderGeneric<SectionController>)(object)((MonoInstallerBase)this).Container.Bind<SectionController>()).FromInstance(sectionController).AsSingle();
		((FromBinderGeneric<RouletteSectionController>)(object)((MonoInstallerBase)this).Container.Bind<RouletteSectionController>()).FromInstance(rouletteSectionController).AsSingle();
		((BindSignalToBinder<ViewUpdateSignal>)(object)SignalExtensions.BindSignal<ViewUpdateSignal>(((MonoInstallerBase)this).Container)).ToMethod<BankWindow>((Func<BankWindow, Action<ViewUpdateSignal>>)((BankWindow x) => x.OnUpdateRequest)).FromResolve();
		((BindSignalToBinder<OpenTabSignal>)(object)SignalExtensions.BindSignal<OpenTabSignal>(((MonoInstallerBase)this).Container)).ToMethod<BankTabUIController>((Func<BankTabUIController, Action<OpenTabSignal>>)((BankTabUIController x) => x.SelectTabBySignal)).FromResolve();
		((FromBinder)((MonoInstallerBase)this).Container.Bind<BankSectionRedirectDispatcher>()).FromNew().AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<GirlPromoOpener>()).AsSingle();
	}
}
