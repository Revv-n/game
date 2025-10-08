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
		((FromBinder)((MonoInstallerBase)this).Container.Bind<RoulettePreviewSmallCardManager>()).FromNewComponentOn(SmallCardContainer.gameObject).AsSingle();
		((FromBinder)((MonoInstallerBase)this).Container.Bind<RoulettePreviewBigCardManager>()).FromNewComponentOn(BigCardContainer.gameObject).AsSingle();
		BindFactories();
	}

	private void BindFactories()
	{
		((ArgConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<ViewFactory<LinkedContent, SmallGirlPromoCardView>>()).AsSingle()).WithArguments<Transform, SmallGirlPromoCardView>(SmallCardContainer, SmallGirlPromoCardView);
		((ArgConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<ViewFactory<LinkedContent, SmallAnyPromoCardView>>()).AsSingle()).WithArguments<Transform, SmallAnyPromoCardView>(SmallCardContainer, SmallAnyPromoCardView);
		((ArgConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<ViewFactory<LinkedContent, BigGirlPromoCardView>>()).AsSingle()).WithArguments<Transform, BigGirlPromoCardView>(BigCardContainer, BigGirlPromoCardView);
		((ArgConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<ViewFactory<LinkedContent, BigAnyPromoCardView>>()).AsSingle()).WithArguments<Transform, BigAnyPromoCardView>(BigCardContainer, BigAnyPromoCardView);
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<RoulettePreviewCardFactory>()).AsSingle();
	}
}
