using GreenT.HornyScapes.Meta.Decorations;
using GreenT.HornyScapes.Presents.Factories;
using Zenject;

namespace GreenT.HornyScapes.Content;

public class ContentInstaller : Installer<ContentInstaller>
{
	public override void InstallBindings()
	{
		base.Container.BindInterfacesTo<BoosterContentFactory>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<CurrencyContentFactory>().AsSingle();
		base.Container.BindInterfacesTo<BattlePassLevelContentFactory>().AsSingle();
		base.Container.BindInterfacesTo<LootboxContentFactory>().AsSingle();
		base.Container.BindInterfacesTo<MergeItemContentFactory>().AsSingle();
		base.Container.BindInterfacesTo<CardContentFactory>().AsSingle();
		base.Container.BindInterfacesTo<ContentFactory>().AsSingle();
		base.Container.BindInterfacesTo<DecorationContentFactory>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<PresentContentFactory>().AsSingle();
	}
}
