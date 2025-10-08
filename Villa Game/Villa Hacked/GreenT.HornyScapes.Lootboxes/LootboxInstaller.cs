using GreenT.HornyScapes.Content;
using GreenT.HornyScapes.Data;
using GreenT.HornyScapes.Lootboxes.Data;
using Zenject;

namespace GreenT.HornyScapes.Lootboxes;

public class LootboxInstaller : Installer<LootboxInstaller>
{
	public override void InstallBindings()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<LootboxCollection>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<DropFactory>()).AsCached();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<LootboxOpener>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<ContentAdder>()).AsCached();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<LootboxStructureInitializer>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<LootboxMapper>>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesTo<LootboxFactory>()).AsCached();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<LinkedContentFactory>()).AsCached();
	}
}
