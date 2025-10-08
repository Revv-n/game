using System;
using StripClub.Model.Shop.Data;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Bank.Offer.UI;

public class PreviewInstaller : MonoInstaller
{
	[SerializeField]
	private OfferPreview preview;

	[SerializeField]
	private OfferSelector selectorPrefab;

	[SerializeField]
	private Transform selectorContainer;

	public override void InstallBindings()
	{
		((FromBinderGeneric<OfferPreview>)(object)((MonoInstallerBase)this).Container.Bind<OfferPreview>()).FromInstance(preview).AsSingle();
		((TransformScopeConcreteIdArgConditionCopyNonLazyBinder)((FactoryFromBinderBase)((MonoInstallerBase)this).Container.BindIFactory<OfferSelector>()).FromComponentInNewPrefab((UnityEngine.Object)selectorPrefab)).UnderTransform(selectorContainer);
		((FromBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<OfferSelector.Manager>()).FromNewComponentOn(selectorContainer.gameObject).AsSingle();
		((NonLazyBinder)((FromBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<OfferMergePreviewController>()).FromNewComponentOn(((Component)this).gameObject).AsSingle()).NonLazy();
		((NonLazyBinder)((FromBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<OfferPaginationController>()).FromNewComponentOn(((Component)this).gameObject).AsSingle()).NonLazy();
		((BindSignalToBinder<LotBoughtSignal>)(object)SignalExtensions.BindSignal<LotBoughtSignal>(((MonoInstallerBase)this).Container)).ToMethod<OfferMergePreviewController>((Func<OfferMergePreviewController, Action<LotBoughtSignal>>)((OfferMergePreviewController x) => x.OnLotBoughtRequest)).FromResolve();
		((BindSignalToBinder<LotBoughtSignal>)(object)SignalExtensions.BindSignal<LotBoughtSignal>(((MonoInstallerBase)this).Container)).ToMethod<OfferPaginationController>((Func<OfferPaginationController, Action<LotBoughtSignal>>)((OfferPaginationController x) => x.OnLotBoughtRequest)).FromResolve();
	}
}
