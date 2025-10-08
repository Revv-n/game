using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Relationships.Views;

public class StatusViewControllerInstaller : MonoInstaller<StatusViewControllerInstaller>
{
	[SerializeField]
	private StatusView _statusViewPrefab;

	[SerializeField]
	private Transform _statusContainer;

	public override void InstallBindings()
	{
		base.Container.BindIFactory<StatusView>().FromComponentInNewPrefab(_statusViewPrefab).UnderTransform(_statusContainer);
		base.Container.BindInterfacesAndSelfTo<StatusViewManager>().FromNewComponentOn(_statusContainer.gameObject).AsSingle();
		base.Container.Bind<StatusViewController>().FromComponentInHierarchy().AsSingle();
	}
}
