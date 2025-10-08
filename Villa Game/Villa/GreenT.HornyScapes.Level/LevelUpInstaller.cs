using GreenT.HornyScapes.Data;
using GreenT.HornyScapes.Level.Data;
using Zenject;

namespace GreenT.HornyScapes.Level;

public class LevelUpInstaller : Installer<LevelUpInstaller>
{
	public override void InstallBindings()
	{
		base.Container.BindInterfacesAndSelfTo<LevelArgsManager>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<LevelArgsStructureInitializer>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<LevelsArgsMapper>>().AsSingle();
		base.Container.BindInterfacesTo<PlayerExperienceController>().AsSingle();
		base.Container.BindInterfacesTo<LevelsArgsFactory>().AsCached();
	}
}
