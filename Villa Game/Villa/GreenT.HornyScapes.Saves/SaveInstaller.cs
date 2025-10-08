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
		base.Container.BindInstance(SaveEventsMainMode).WithId(SaveMode.Main).WhenInjectedInto<ClusterFactory>();
		base.Container.BindInstance(SaveEventsTutorialMode).WithId(SaveMode.Tutorial).WhenInjectedInto<ClusterFactory>();
		base.Container.BindInterfacesAndSelfTo<SaveEventClusterManager>().FromFactory<SaveEventClusterManager, ClusterFactory>().AsSingle();
	}

	private void BindSelector()
	{
		base.Container.BindInterfacesAndSelfTo<SaveModeSelector>().AsSingle().WithArguments("tutorial_save_mode_switch_off");
	}

	private void BindSaveSystem()
	{
		base.Container.BindInterfacesAndSelfTo<SaveProviderFacade>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<InitializeState>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<PostLoadingState>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<PreloadContentService>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<SaveController>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<SaveNotifier>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<Saver>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<AuthorizationProcessorService>().AsSingle();
		base.Container.Bind<ServerSaveProvider>().AsSingle();
		base.Container.Bind<StringSerializer>().AsSingle();
		base.Container.Bind<SaveLoader>().AsSingle();
		base.Container.Bind<SaverState>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<SaveSender>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<FileSerializer>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<MigrateToNewBalance>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<MigrationDeleteMissingItems>().AsSingle();
		base.Container.Bind<ManualSave>().AsSingle();
	}
}
