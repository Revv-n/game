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
		((TransformScopeConcreteIdArgConditionCopyNonLazyBinder)((FactoryFromBinderBase)((MonoInstallerBase)this).Container.BindIFactory<StatusView>()).FromComponentInNewPrefab((Object)_statusViewPrefab)).UnderTransform(_statusContainer);
		((FromBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<StatusViewManager>()).FromNewComponentOn(_statusContainer.gameObject).AsSingle();
		((FromBinder)((MonoInstallerBase)this).Container.Bind<StatusViewController>()).FromComponentInHierarchy(true).AsSingle();
	}
}
