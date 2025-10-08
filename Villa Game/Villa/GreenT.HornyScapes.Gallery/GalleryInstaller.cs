using GreenT.HornyScapes.Data;
using GreenT.HornyScapes.Gallery.Data;
using StripClub.Gallery.Data;
using Zenject;

namespace GreenT.HornyScapes.Gallery;

public class GalleryInstaller : Installer<GalleryInstaller>
{
	public override void InstallBindings()
	{
		base.Container.BindInterfacesTo<ConfigDataLoader<MediaMapper>>().AsSingle();
		base.Container.Bind<MapperStructureInitializer<MediaMapper>>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<MediaMapper.Manager>().AsSingle();
		base.Container.BindInterfacesTo<Gallery>().AsSingle();
		base.Container.BindInterfacesTo<MediaDataLoader>().AsSingle();
		base.Container.BindInterfacesTo<MediaFactory>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<MediaInfoInitializer>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<MediaMapper>>().AsSingle();
		base.Container.Bind<GalleryController>().FromNew().AsSingle();
		base.Container.Bind<GalleryState>().FromFactory<StateFactory>().AsSingle();
	}
}
