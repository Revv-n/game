using GreenT.HornyScapes.StarShop.Story;
using GreenT.HornyScapes.StarShop.UI;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Tasks.UI;

public class TaskWindowInstall : MonoInstaller<TaskWindowInstall>
{
	[SerializeField]
	private TaskBook taskWindow;

	[SerializeField]
	private StarShopView starShopViewPrefab;

	[SerializeField]
	private Transform shopViewContainer;

	[SerializeField]
	private TaskToggleBlock taskToggleBlock;

	public override void InstallBindings()
	{
		((FromBinderGeneric<TaskBook>)(object)((MonoInstallerBase)this).Container.Bind<TaskBook>()).FromInstance(taskWindow).AsSingle();
		BindStartShops();
		BindAnimations();
	}

	private void BindStartShops()
	{
		((NonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesTo<ArtLoadController>()).AsSingle()).NonLazy();
		BindEntity<StarShopView, StarShopViewManager>(starShopViewPrefab, shopViewContainer);
	}

	private void BindAnimations()
	{
		((FromBinderGeneric<TaskToggleBlock>)(object)((MonoInstallerBase)this).Container.Bind<TaskToggleBlock>()).FromInstance(taskToggleBlock);
	}

	private void BindEntity<TEntity, TManager>(TEntity prefab, Transform container) where TEntity : MonoBehaviour
	{
		((TransformScopeConcreteIdArgConditionCopyNonLazyBinder)((FactoryFromBinderBase)((MonoInstallerBase)this).Container.BindIFactory<TEntity>()).FromComponentInNewPrefab((Object)prefab)).UnderTransform(container);
		((FromBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<TManager>()).FromNewComponentOn(container.gameObject).AsSingle();
	}
}
