using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Tasks.UI;

public class TaskSubInstaller : MonoInstaller<TaskSubInstaller>
{
	public ObjectiveView prefab;

	public Transform subItemContainer;

	public override void InstallBindings()
	{
		((TransformScopeConcreteIdArgConditionCopyNonLazyBinder)((FactoryFromBinderBase)((MonoInstallerBase)this).Container.BindIFactory<ObjectiveView>()).FromComponentInNewPrefab((Object)prefab)).UnderTransform(subItemContainer).AsCached();
		((FromBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<ObjectiveViewManager>()).FromNewComponentOn(subItemContainer.gameObject).AsSingle();
	}
}
