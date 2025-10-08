using GreenT.HornyScapes.Data;
using Zenject;

namespace GreenT.HornyScapes.GameItems;

public class GameItemInstaller : Installer<GameItemInstaller>
{
	public override void InstallBindings()
	{
		base.Container.BindInterfacesAndSelfTo<GameItemManager>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<GameItemMapper>>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<GameItemStructureInitializer>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<GameItemConfigFactory>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<GameItemConfigManager>().AsSingle();
	}
}
