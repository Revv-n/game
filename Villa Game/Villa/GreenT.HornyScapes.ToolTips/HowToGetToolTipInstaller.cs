using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.ToolTips;

public class HowToGetToolTipInstaller : MonoInstaller<HowToGetToolTipInstaller>
{
	[SerializeField]
	private ItemGetView viewCreator;

	[SerializeField]
	private MergeItemCollectionView prefab;

	[SerializeField]
	private Transform viewContainer;

	public override void InstallBindings()
	{
		base.Container.Bind<ItemGetView>().FromInstance(viewCreator).AsSingle();
		base.Container.BindIFactory<MergeItemCollectionView>().FromComponentInNewPrefab(prefab).UnderTransform(viewContainer)
			.AsCached();
		base.Container.BindInterfacesAndSelfTo<MergeItemCollectionView.Manager>().FromNewComponentOn(viewContainer.gameObject).AsSingle();
	}
}
