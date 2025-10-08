using GreenT.HornyScapes.Data;
using Zenject;

namespace GreenT.HornyScapes.Meta.Duplicates;

public class DuplicateRewardInstaller : Installer<DuplicateRewardInstaller>
{
	public override void InstallBindings()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<DuplicateRewardMapper>>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<DuplicateRewardStructureInitializer>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<DuplicateRewardsManager>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesTo<DuplicateRewardsFactory>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.Bind<DuplicateRewardProvider>()).AsSingle();
	}
}
