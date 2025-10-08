using GreenT.HornyScapes.Bank.Offer.UI;
using GreenT.HornyScapes.Tasks.UI;
using GreenT.HornyScapes.ToolTips;
using Merge;
using Merge.MotionDesign;
using StripClub.UI;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.MergeCore;

public class MergeSceneInstaller : MonoInstaller<MergeSceneInstaller>
{
	public TaskStarFactory TaskStarFactory;

	public TaskStarManager TaskStarManager;

	public TaskJewelFactory TaskJewelFactory;

	public TaskJewelManager TaskJewelManager;

	public TaskContractsFactory TaskContractsFactory;

	public TaskContractsManager TaskContractsManager;

	public Transform TaskViewContainer;

	public TaskView TaskViewPrefab;

	public InputController InputController;

	public TaskStarFly taskStarFly;

	public PocketController PocketController;

	[SerializeField]
	private PreviewInstaller offerPreviewPrefab;

	[SerializeField]
	private Transform offerPreviewParent;

	[SerializeField]
	public CollectionController collectionController;

	[Header("Tips")]
	public HowToGetToolTipView prefab;

	public Transform container;

	public override void InstallBindings()
	{
		BindControllers();
		BindTaskMergeView();
		BindStarAnimation();
		BindOffers();
		BindMainFactory();
		base.Container.BindInterfacesAndSelfTo<InputController>().FromInstance(InputController).AsSingle();
		BindTooltip<HowToGetToolTipView, HowToGetToolTipSettings, HowToGetToolTipView.Manager>(prefab, container.transform);
	}

	private void BindTooltip<TView, TSettings, TManager>(HowToGetToolTipView prefab, Transform parent) where TView : IView<TSettings> where TSettings : ToolTipSettings where TManager : IViewManager<TSettings, TView>
	{
		base.Container.BindIFactory<TView>().FromComponentInNewPrefab(prefab).UnderTransform(parent);
		base.Container.BindInterfacesTo<TManager>().FromNewComponentOn(parent.gameObject).AsSingle();
	}

	private void BindOffers()
	{
		base.Container.Bind<PreviewInstaller>().FromComponentInNewPrefab(offerPreviewPrefab).UnderTransform(offerPreviewParent)
			.AsSingle()
			.NonLazy();
	}

	private void BindMainFactory()
	{
		base.Container.BindInterfacesAndSelfTo<TaskStarFactory>().FromInstance(TaskStarFactory).AsSingle();
		base.Container.BindInterfacesAndSelfTo<TaskJewelFactory>().FromInstance(TaskJewelFactory).AsSingle();
		base.Container.BindInterfacesAndSelfTo<TaskContractsFactory>().FromInstance(TaskContractsFactory).AsSingle();
		base.Container.BindInterfacesAndSelfTo<TaskStarManager>().FromInstance(TaskStarManager).AsSingle();
		base.Container.BindInterfacesAndSelfTo<TaskJewelManager>().FromInstance(TaskJewelManager).AsSingle();
		base.Container.BindInterfacesAndSelfTo<TaskContractsManager>().FromInstance(TaskContractsManager).AsSingle();
		base.Container.Bind<TaskRewardsMainFactory>().AsSingle();
	}

	private void BindStarAnimation()
	{
		base.Container.Bind<TaskStarFly>().FromInstance(taskStarFly);
	}

	private void BindTaskMergeView()
	{
		base.Container.BindIFactory<TaskView>().FromComponentInNewPrefab(TaskViewPrefab).UnderTransform(TaskViewContainer)
			.AsSingle();
		base.Container.BindInterfacesAndSelfTo<MergeTaskViewManager>().FromNewComponentOn(TaskViewContainer.gameObject).AsSingle();
	}

	private void BindControllers()
	{
		base.Container.BindInterfacesAndSelfTo<CollectionController>().FromInstance(collectionController);
		base.Container.BindInterfacesAndSelfTo<PocketController>().FromInstance(PocketController).AsCached();
	}
}
