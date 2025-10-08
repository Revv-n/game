using GreenT.HornyScapes.MiniEvents;
using StripClub.Model;
using StripClub.UI;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes;

public sealed class RouletteInstaller : MonoInstaller<RouletteInstaller>
{
	public SmallGirlPromoCardView SmallGirlPromoCardView;

	public SmallAnyPromoCardView SmallAnyPromoCardView;

	public BigGirlPromoCardView BigGirlPromoCardView;

	public BigAnyPromoCardView BigAnyPromoCardView;

	public Transform SmallCardContainer;

	public Transform BigCardContainer;

	public override void InstallBindings()
	{
		base.Container.Bind<RoulettePreviewSmallCardManager>().FromNewComponentOn(SmallCardContainer.gameObject).AsSingle();
		base.Container.Bind<RoulettePreviewBigCardManager>().FromNewComponentOn(BigCardContainer.gameObject).AsSingle();
		BindFactories();
	}

	private void BindFactories()
	{
		base.Container.BindInterfacesAndSelfTo<ViewFactory<LinkedContent, SmallGirlPromoCardView>>().AsSingle().WithArguments(SmallCardContainer, SmallGirlPromoCardView);
		base.Container.BindInterfacesAndSelfTo<ViewFactory<LinkedContent, SmallAnyPromoCardView>>().AsSingle().WithArguments(SmallCardContainer, SmallAnyPromoCardView);
		base.Container.BindInterfacesAndSelfTo<ViewFactory<LinkedContent, BigGirlPromoCardView>>().AsSingle().WithArguments(BigCardContainer, BigGirlPromoCardView);
		base.Container.BindInterfacesAndSelfTo<ViewFactory<LinkedContent, BigAnyPromoCardView>>().AsSingle().WithArguments(BigCardContainer, BigAnyPromoCardView);
		base.Container.Bind<RoulettePreviewCardFactory>().AsSingle();
	}
}
