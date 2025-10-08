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
		base.Container.BindInterfacesAndSelfTo<SpawnerReloader>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<InfoGetItem>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<MixerReloader>().AsSingle();
	}

	private void BindCore()
	{
		base.Container.Bind<MergeNotifier>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<MergeFieldFactory>().AsCached();
		base.Container.BindInterfacesAndSelfTo<MergeFieldProvider>().AsCached();
		base.Container.BindInterfacesAndSelfTo<MergeFieldManager>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<MergeFieldInitializer>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<MergeFieldMapper>>().AsSingle();
	}

	private void BindRecipes()
	{
		base.Container.BindInterfacesAndSelfTo<RecipeManager>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<RecipeFactory>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<RecipeMapperManager>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<RecipeStructureInitializer>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<RecipeMapper>>().AsSingle();
	}
}
