using System;
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
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventTaskManager>()).AsSingle();
	}

	private void BindMiniEventStructure()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<TaskActivityStructureInitializer<MiniEventTask>>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<StructureInitializerProxyWithArrayFromConfig<TaskActivityMapper>>()).AsSingle();
	}

	private void BindMiniEventFactory()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventTaskFactory>()).AsSingle();
	}

	private void BindManager(ContentType contentType)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Expected O, but got Unknown
		((ConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesTo<TasksManager>().FromResolveGetter<TaskManagerCluster, TasksManager>((Func<TaskManagerCluster, TasksManager>)((TaskManagerCluster cluster) => cluster[contentType])).AsCached()
			.WithConcreteId((object)contentType)).When((BindingCondition)((InjectContext context) => context.ObjectType == typeof(TaskStructureInitializer<Task>) && contentType.Equals(context.ConcreteIdentifier)));
	}

	private void BindStructure(ContentType contentType)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected O, but got Unknown
		((ConditionCopyNonLazyBinder)((ConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<TaskStructureInitializer<Task>>()).WithConcreteId((object)contentType)).When((BindingCondition)((InjectContext context) => context.ObjectType == typeof(StructureInitializerProxyWithArrayFromConfig<TaskMapper>) && contentType.Equals(context.ParentContext.Identifier)));
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<StructureInitializerProxyWithArrayFromConfig<TaskMapper>>().WithId((object)contentType)).AsCached();
	}

	private void BindFactory()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesTo<TaskFactory>()).AsCached();
	}

	private void BindFactories()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<TaskStateFactory>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<MiniEventTaskStateFactory>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<GoalFactory>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<ObjectiveFactory>()).AsSingle();
		((FactoryFromBinderBase)((MonoInstallerBase)this).Container.BindIFactory<TasksManager>()).FromNew();
		((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<TaskManagerCluster>().FromFactory<TaskManagerCluster, ClusterFactory<TasksManager, TaskManagerCluster>>().AsSingle();
	}

	private void BindController()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInstance<TaskObjectiveIcons>(TaskObjectiveIcons)).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<TaskController>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<TaskContentTypeEnumConverter>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<TaskStateProvider>()).AsSingle();
	}

	private void BindObjectiveFactories()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<CurrencyObjectiveFactory>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<SummonObjectiveFactory>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<PromoteObjectiveFactory>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<GetCardObjectiveFactory>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MergeObjectiveFactory>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<PhotoObjectiveFactory>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<CompletedDialogueObjectiveFactory>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<CompletedDialogueAnswersObjectiveFactory>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<RouletteObjectiveFactory>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<BattlePassObjectiveFactory>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<PresentObjectiveFactory>()).AsSingle();
	}
}
