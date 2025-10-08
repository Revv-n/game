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
		((FromBinderGeneric<ItemGetView>)(object)((MonoInstallerBase)this).Container.Bind<ItemGetView>()).FromInstance(viewCreator).AsSingle();
		((TransformScopeConcreteIdArgConditionCopyNonLazyBinder)((FactoryFromBinderBase)((MonoInstallerBase)this).Container.BindIFactory<MergeItemCollectionView>()).FromComponentInNewPrefab((Object)prefab)).UnderTransform(viewContainer).AsCached();
		((FromBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MergeItemCollectionView.Manager>()).FromNewComponentOn(viewContainer.gameObject).AsSingle();
	}
}
