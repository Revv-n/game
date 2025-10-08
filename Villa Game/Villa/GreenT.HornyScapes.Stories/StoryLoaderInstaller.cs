using GreenT.HornyScapes.Data;
using GreenT.HornyScapes.Events.Content;
using GreenT.HornyScapes.Stories.Data;
using Zenject;

namespace GreenT.HornyScapes.Stories;

public class StoryLoaderInstaller : Installer<StoryLoaderInstaller>
{
	public override void InstallBindings()
	{
		base.Container.BindIFactory<StoryManager>().FromNew();
		base.Container.BindInterfacesAndSelfTo<StoryCluster>().FromFactory<StoryCluster, ClusterFactory<StoryManager, StoryCluster>>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<StroyStructureInitializer>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<PhraseMapper>>().AsSingle();
		base.Container.BindInterfacesTo<PhraseFactory>().AsCached();
		base.Container.Bind<StoryManager>().FromNew().AsSingle();
	}
}
