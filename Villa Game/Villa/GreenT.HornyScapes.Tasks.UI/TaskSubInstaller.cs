using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Tasks.UI;

public class TaskSubInstaller : MonoInstaller<TaskSubInstaller>
{
	public ObjectiveView prefab;

	public Transform subItemContainer;

	public override void InstallBindings()
	{
		base.Container.BindIFactory<ObjectiveView>().FromComponentInNewPrefab(prefab).UnderTransform(subItemContainer)
			.AsCached();
		base.Container.BindInterfacesAndSelfTo<ObjectiveViewManager>().FromNewComponentOn(subItemContainer.gameObject).AsSingle();
	}
}
