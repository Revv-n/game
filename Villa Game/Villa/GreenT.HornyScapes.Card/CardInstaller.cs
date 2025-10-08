using GreenT.HornyScapes.Card.Data;
using GreenT.HornyScapes.Data;
using GreenT.Multiplier;
using StripClub.Model.Cards;
using StripClub.Model.Data;
using Zenject;

namespace GreenT.HornyScapes.Card;

public class CardInstaller : Installer<CardInstaller>
{
	public override void InstallBindings()
	{
		base.Container.Bind<MultiplierManager>().AsSingle();
		base.Container.Bind<PromotePatterns>().AsSingle();
		base.Container.BindInterfacesTo<PromotePatternsFactory>().AsSingle();
		base.Container.BindInterfacesTo<PromoteInfoLoader>().AsSingle();
		base.Container.Bind<CardsCollection>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<CardsCollectionTracker>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<PromotePatternsStructureInitializer>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<PromotePatternMapper>>().AsSingle();
		base.Container.BindInterfacesTo<PromoteFactory>().AsSingle();
	}
}
