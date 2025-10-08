using GreenT.HornyScapes.Data;
using Zenject;

namespace GreenT.HornyScapes.Meta.Duplicates;

public class DuplicateRewardInstaller : Installer<DuplicateRewardInstaller>
{
	public override void InstallBindings()
	{
		base.Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<DuplicateRewardMapper>>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<DuplicateRewardStructureInitializer>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<DuplicateRewardsManager>().AsSingle();
		base.Container.BindInterfacesTo<DuplicateRewardsFactory>().AsSingle();
		base.Container.Bind<DuplicateRewardProvider>().AsSingle();
	}
}
