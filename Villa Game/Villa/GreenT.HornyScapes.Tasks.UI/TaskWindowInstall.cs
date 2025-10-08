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
		base.Container.Bind<TaskBook>().FromInstance(taskWindow).AsSingle();
		BindStartShops();
		BindAnimations();
	}

	private void BindStartShops()
	{
		base.Container.BindInterfacesTo<ArtLoadController>().AsSingle().NonLazy();
		BindEntity<StarShopView, StarShopViewManager>(starShopViewPrefab, shopViewContainer);
	}

	private void BindAnimations()
	{
		base.Container.Bind<TaskToggleBlock>().FromInstance(taskToggleBlock);
	}

	private void BindEntity<TEntity, TManager>(TEntity prefab, Transform container) where TEntity : MonoBehaviour
	{
		base.Container.BindIFactory<TEntity>().FromComponentInNewPrefab(prefab).UnderTransform(container);
		base.Container.BindInterfacesAndSelfTo<TManager>().FromNewComponentOn(container.gameObject).AsSingle();
	}
}
