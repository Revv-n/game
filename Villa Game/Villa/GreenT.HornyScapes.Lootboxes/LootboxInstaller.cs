using GreenT.HornyScapes.Content;
using GreenT.HornyScapes.Data;
using GreenT.HornyScapes.Lootboxes.Data;
using Zenject;

namespace GreenT.HornyScapes.Lootboxes;

public class LootboxInstaller : Installer<LootboxInstaller>
{
	public override void InstallBindings()
	{
		base.Container.BindInterfacesAndSelfTo<LootboxCollection>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<DropFactory>().AsCached();
		base.Container.BindInterfacesAndSelfTo<LootboxOpener>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<ContentAdder>().AsCached();
		base.Container.BindInterfacesAndSelfTo<LootboxStructureInitializer>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<LootboxMapper>>().AsSingle();
		base.Container.BindInterfacesTo<LootboxFactory>().AsCached();
		base.Container.BindInterfacesAndSelfTo<LinkedContentFactory>().AsCached();
	}
}
