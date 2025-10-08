using GreenT.HornyScapes.Data;
using GreenT.HornyScapes.GameItems;
using GreenT.HornyScapes.MergeField;
using Zenject;

namespace GreenT.HornyScapes.MergeCore;

public class MergeInstaller : Installer<MergeInstaller>
{
	public override void InstallBindings()
	{
		BindCore();
		BindRecipes();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<SpawnerReloader>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<InfoGetItem>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<MixerReloader>()).AsSingle();
	}

	private void BindCore()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.Bind<MergeNotifier>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<MergeFieldFactory>()).AsCached();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<MergeFieldProvider>()).AsCached();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<MergeFieldManager>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<MergeFieldInitializer>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<MergeFieldMapper>>()).AsSingle();
	}

	private void BindRecipes()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<RecipeManager>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<RecipeFactory>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<RecipeMapperManager>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<RecipeStructureInitializer>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<RecipeMapper>>()).AsSingle();
	}
}
