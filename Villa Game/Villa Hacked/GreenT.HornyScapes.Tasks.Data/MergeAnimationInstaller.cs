using System;
using GreenT.HornyScapes.Events.Content;
using GreenT.HornyScapes.MergeCore;
using GreenT.Types;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Tasks.Data;

public class MergeAnimationInstaller : MonoInstaller<MergeAnimationInstaller>
{
	public CloudParticle cloudPrefab;

	public Transform containerCloud;

	public TaskCollectAnimationManager mainCloudViewManager;

	public TaskCollectAnimationManager eventCloudViewManager;

	public override void InstallBindings()
	{
		BindContentSelector();
		PassTaskAnimation();
	}

	private void PassTaskAnimation()
	{
		((NonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<TaskCollect>()).AsSingle()).NonLazy();
		((TransformScopeConcreteIdArgConditionCopyNonLazyBinder)((FactoryFromBinderBase)((MonoInstallerBase)this).Container.BindIFactory<CloudParticle>()).FromComponentInNewPrefab((UnityEngine.Object)cloudPrefab)).UnderTransform(containerCloud).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<TaskCollectContentCluster>()).AsSingle();
		AddManagerToCluster(ContentType.Main, mainCloudViewManager);
		AddManagerToCluster(ContentType.Event, eventCloudViewManager);
	}

	private void AddManagerToCluster(ContentType typeId, TaskCollectAnimationManager manager)
	{
		((FromBinderGeneric<TaskCollectAnimationManager>)(object)((MonoInstallerBase)this).Container.Bind<TaskCollectAnimationManager>().WithId((object)typeId)).FromInstance(manager).AsCached();
	}

	private void BindContentSelector()
	{
		((NonLazyBinder)((InstantiateCallbackConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MergeAnimationSelector>()).AsSingle()).OnInstantiated<MergeAnimationSelector>((Action<InjectContext, MergeAnimationSelector>)ContentSelectorInstaller.AddSelectorToContainer)).NonLazy();
	}
}
