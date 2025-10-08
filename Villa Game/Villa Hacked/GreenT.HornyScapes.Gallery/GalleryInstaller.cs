using GreenT.HornyScapes.Data;
using GreenT.HornyScapes.Gallery.Data;
using StripClub.Gallery.Data;
using Zenject;

namespace GreenT.HornyScapes.Gallery;

public class GalleryInstaller : Installer<GalleryInstaller>
{
	public override void InstallBindings()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesTo<ConfigDataLoader<MediaMapper>>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.Bind<MapperStructureInitializer<MediaMapper>>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<MediaMapper.Manager>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesTo<Gallery>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesTo<MediaDataLoader>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesTo<MediaFactory>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<MediaInfoInitializer>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<MediaMapper>>()).AsSingle();
		((FromBinder)((InstallerBase)this).Container.Bind<GalleryController>()).FromNew().AsSingle();
		((FromBinderGeneric<GalleryState>)(object)((InstallerBase)this).Container.Bind<GalleryState>()).FromFactory<StateFactory>().AsSingle();
	}
}
