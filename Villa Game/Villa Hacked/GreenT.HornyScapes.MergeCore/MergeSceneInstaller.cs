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
		((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<InputController>().FromInstance((object)InputController).AsSingle();
		BindTooltip<HowToGetToolTipView, HowToGetToolTipSettings, HowToGetToolTipView.Manager>(prefab, container.transform);
	}

	private void BindTooltip<TView, TSettings, TManager>(HowToGetToolTipView prefab, Transform parent) where TView : IView<TSettings> where TSettings : ToolTipSettings where TManager : IViewManager<TSettings, TView>
	{
		((TransformScopeConcreteIdArgConditionCopyNonLazyBinder)((FactoryFromBinderBase)((MonoInstallerBase)this).Container.BindIFactory<TView>()).FromComponentInNewPrefab((Object)prefab)).UnderTransform(parent);
		((FromBinder)((MonoInstallerBase)this).Container.BindInterfacesTo<TManager>()).FromNewComponentOn(parent.gameObject).AsSingle();
	}

	private void BindOffers()
	{
		((NonLazyBinder)((TransformScopeConcreteIdArgConditionCopyNonLazyBinder)((FromBinder)((MonoInstallerBase)this).Container.Bind<PreviewInstaller>()).FromComponentInNewPrefab((Object)(object)offerPreviewPrefab)).UnderTransform(offerPreviewParent).AsSingle()).NonLazy();
	}

	private void BindMainFactory()
	{
		((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<TaskStarFactory>().FromInstance((object)TaskStarFactory).AsSingle();
		((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<TaskJewelFactory>().FromInstance((object)TaskJewelFactory).AsSingle();
		((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<TaskContractsFactory>().FromInstance((object)TaskContractsFactory).AsSingle();
		((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<TaskStarManager>().FromInstance((object)TaskStarManager).AsSingle();
		((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<TaskJewelManager>().FromInstance((object)TaskJewelManager).AsSingle();
		((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<TaskContractsManager>().FromInstance((object)TaskContractsManager).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<TaskRewardsMainFactory>()).AsSingle();
	}

	private void BindStarAnimation()
	{
		((FromBinderGeneric<TaskStarFly>)(object)((MonoInstallerBase)this).Container.Bind<TaskStarFly>()).FromInstance(taskStarFly);
	}

	private void BindTaskMergeView()
	{
		((TransformScopeConcreteIdArgConditionCopyNonLazyBinder)((FactoryFromBinderBase)((MonoInstallerBase)this).Container.BindIFactory<TaskView>()).FromComponentInNewPrefab((Object)TaskViewPrefab)).UnderTransform(TaskViewContainer).AsSingle();
		((FromBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MergeTaskViewManager>()).FromNewComponentOn(TaskViewContainer.gameObject).AsSingle();
	}

	private void BindControllers()
	{
		((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<CollectionController>().FromInstance((object)collectionController);
		((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<PocketController>().FromInstance((object)PocketController).AsCached();
	}
}
