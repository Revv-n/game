using System.Collections.Generic;
using GreenT.Data;
using StripClub.NewEvent.Data;
using Zenject;

namespace GreenT.HornyScapes.Saves;

public class SaveInstaller : MonoInstaller<SaveInstaller>
{
	private const string RemoteProviderKey = "Remote Provider";

	private const string SwitchToMainModeLocker = "tutorial_save_mode_switch_off";

	public List<SaveEvent> SaveEventsMainMode = new List<SaveEvent>();

	public List<SaveEvent> SaveEventsTutorialMode = new List<SaveEvent>();

	public override void InstallBindings()
	{
		BindSaveSystem();
		BindCluster();
		BindSelector();
	}

	private void BindCluster()
	{
		((ConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInstance<List<SaveEvent>>(SaveEventsMainMode).WithId((object)SaveMode.Main)).WhenInjectedInto<ClusterFactory>();
		((ConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInstance<List<SaveEvent>>(SaveEventsTutorialMode).WithId((object)SaveMode.Tutorial)).WhenInjectedInto<ClusterFactory>();
		((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<SaveEventClusterManager>().FromFactory<SaveEventClusterManager, ClusterFactory>().AsSingle();
	}

	private void BindSelector()
	{
		((ArgConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<SaveModeSelector>()).AsSingle()).WithArguments<string>("tutorial_save_mode_switch_off");
	}

	private void BindSaveSystem()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<SaveProviderFacade>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<InitializeState>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<PostLoadingState>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<PreloadContentService>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<SaveController>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<SaveNotifier>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<Saver>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<AuthorizationProcessorService>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<ServerSaveProvider>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<StringSerializer>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<SaveLoader>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<SaverState>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<SaveSender>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<FileSerializer>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MigrateToNewBalance>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MigrationDeleteMissingItems>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<ManualSave>()).AsSingle();
	}
}
