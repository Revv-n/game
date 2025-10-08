using GreenT.HornyScapes.Data;
using GreenT.HornyScapes.Events.Content;
using GreenT.HornyScapes.Extensions;
using GreenT.HornyScapes.MiniEvents;
using GreenT.Types;
using Zenject;

namespace GreenT.HornyScapes.Tasks.Data;

public class TaskInstaller : MonoInstaller<TaskInstaller>
{
	public TaskObjectiveIcons TaskObjectiveIcons;

	public override void InstallBindings()
	{
		BindManagers();
		BindFactory();
		BindObjectiveFactories();
		BindFactories();
		BindController();
		BindMiniEventManager();
		BindMiniEventStructure();
		BindMiniEventFactory();
	}

	private void BindManagers()
	{
		EnumExtension.ForeachEnum(delegate(ContentType contentType)
		{
			BindManager(contentType);
			BindStructure(contentType);
		});
	}

	private void BindMiniEventManager()
	{
		base.Container.BindInterfacesAndSelfTo<MiniEventTaskManager>().AsSingle();
	}

	private void BindMiniEventStructure()
	{
		base.Container.BindInterfacesAndSelfTo<TaskActivityStructureInitializer<MiniEventTask>>().AsSingle();
		base.Container.Bind<StructureInitializerProxyWithArrayFromConfig<TaskActivityMapper>>().AsSingle();
	}

	private void BindMiniEventFactory()
	{
		base.Container.BindInterfacesAndSelfTo<MiniEventTaskFactory>().AsSingle();
	}

	private void BindManager(ContentType contentType)
	{
		base.Container.BindInterfacesTo<TasksManager>().FromResolveGetter((TaskManagerCluster cluster) => cluster[contentType]).AsCached()
			.WithConcreteId(contentType)
			.When((InjectContext context) => context.ObjectType == typeof(TaskStructureInitializer<Task>) && contentType.Equals(context.ConcreteIdentifier));
	}

	private void BindStructure(ContentType contentType)
	{
		base.Container.BindInterfacesAndSelfTo<TaskStructureInitializer<Task>>().WithConcreteId(contentType).When((InjectContext context) => context.ObjectType == typeof(StructureInitializerProxyWithArrayFromConfig<TaskMapper>) && contentType.Equals(context.ParentContext.Identifier));
		base.Container.Bind<StructureInitializerProxyWithArrayFromConfig<TaskMapper>>().WithId(contentType).AsCached();
	}

	private void BindFactory()
	{
		base.Container.BindInterfacesTo<TaskFactory>().AsCached();
	}

	private void BindFactories()
	{
		base.Container.Bind<TaskStateFactory>().AsSingle();
		base.Container.Bind<MiniEventTaskStateFactory>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<GoalFactory>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<ObjectiveFactory>().AsSingle();
		base.Container.BindIFactory<TasksManager>().FromNew();
		base.Container.BindInterfacesAndSelfTo<TaskManagerCluster>().FromFactory<TaskManagerCluster, ClusterFactory<TasksManager, TaskManagerCluster>>().AsSingle();
	}

	private void BindController()
	{
		base.Container.BindInstance(TaskObjectiveIcons).AsSingle();
		base.Container.BindInterfacesAndSelfTo<TaskController>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<TaskContentTypeEnumConverter>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<TaskStateProvider>().AsSingle();
	}

	private void BindObjectiveFactories()
	{
		base.Container.BindInterfacesAndSelfTo<CurrencyObjectiveFactory>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<SummonObjectiveFactory>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<PromoteObjectiveFactory>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<GetCardObjectiveFactory>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<MergeObjectiveFactory>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<PhotoObjectiveFactory>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<CompletedDialogueObjectiveFactory>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<CompletedDialogueAnswersObjectiveFactory>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<RouletteObjectiveFactory>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<BattlePassObjectiveFactory>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<PresentObjectiveFactory>().AsSingle();
	}
}
