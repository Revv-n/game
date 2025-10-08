using GreenT.HornyScapes.Data;
using GreenT.HornyScapes.Events.Content;
using GreenT.HornyScapes.Stories.Data;
using Zenject;

namespace GreenT.HornyScapes.Stories;

public class StoryLoaderInstaller : Installer<StoryLoaderInstaller>
{
	public override void InstallBindings()
	{
		((FactoryFromBinderBase)((InstallerBase)this).Container.BindIFactory<StoryManager>()).FromNew();
		((InstallerBase)this).Container.BindInterfacesAndSelfTo<StoryCluster>().FromFactory<StoryCluster, ClusterFactory<StoryManager, StoryCluster>>().AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<StroyStructureInitializer>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<PhraseMapper>>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesTo<PhraseFactory>()).AsCached();
		((FromBinder)((InstallerBase)this).Container.Bind<StoryManager>()).FromNew().AsSingle();
	}
}
