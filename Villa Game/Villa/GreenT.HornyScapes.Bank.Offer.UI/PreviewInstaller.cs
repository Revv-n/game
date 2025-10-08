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
		base.Container.Bind<OfferPreview>().FromInstance(preview).AsSingle();
		base.Container.BindIFactory<OfferSelector>().FromComponentInNewPrefab(selectorPrefab).UnderTransform(selectorContainer);
		base.Container.BindInterfacesAndSelfTo<OfferSelector.Manager>().FromNewComponentOn(selectorContainer.gameObject).AsSingle();
		base.Container.BindInterfacesAndSelfTo<OfferMergePreviewController>().FromNewComponentOn(base.gameObject).AsSingle()
			.NonLazy();
		base.Container.BindInterfacesAndSelfTo<OfferPaginationController>().FromNewComponentOn(base.gameObject).AsSingle()
			.NonLazy();
		base.Container.BindSignal<LotBoughtSignal>().ToMethod((OfferMergePreviewController x) => x.OnLotBoughtRequest).FromResolve();
		base.Container.BindSignal<LotBoughtSignal>().ToMethod((OfferPaginationController x) => x.OnLotBoughtRequest).FromResolve();
	}
}
