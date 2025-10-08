using GreenT.HornyScapes.Characters.Skins.Content;
using GreenT.HornyScapes.Data;
using Zenject;

namespace GreenT.HornyScapes.Characters.Skins;

public class SkinInstaller : Installer<SkinInstaller>
{
	public override void InstallBindings()
	{
		base.Container.BindInterfacesAndSelfTo<SkinFactory>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<SkinManager>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<SkinStructureInitializer>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<SkinMapper>>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<SkinContentFactory>().AsSingle();
		base.Container.Bind<SkinLoader>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<SkinDataLoadingController>().AsSingle();
	}
}
