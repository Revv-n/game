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
		base.Container.BindInterfacesAndSelfTo<TaskCollect>().AsSingle().NonLazy();
		base.Container.BindIFactory<CloudParticle>().FromComponentInNewPrefab(cloudPrefab).UnderTransform(containerCloud)
			.AsSingle();
		base.Container.BindInterfacesAndSelfTo<TaskCollectContentCluster>().AsSingle();
		AddManagerToCluster(ContentType.Main, mainCloudViewManager);
		AddManagerToCluster(ContentType.Event, eventCloudViewManager);
	}

	private void AddManagerToCluster(ContentType typeId, TaskCollectAnimationManager manager)
	{
		base.Container.Bind<TaskCollectAnimationManager>().WithId(typeId).FromInstance(manager)
			.AsCached();
	}

	private void BindContentSelector()
	{
		base.Container.BindInterfacesAndSelfTo<MergeAnimationSelector>().AsSingle().OnInstantiated<MergeAnimationSelector>(ContentSelectorInstaller.AddSelectorToContainer)
			.NonLazy();
	}
}
