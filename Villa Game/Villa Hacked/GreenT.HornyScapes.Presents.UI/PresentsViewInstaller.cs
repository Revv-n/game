using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Presents.UI;

public class PresentsViewInstaller : MonoInstaller
{
	[SerializeField]
	private PresentView _viewPrefab;

	[SerializeField]
	private Transform _viewContainer;

	public override void InstallBindings()
	{
		((ConditionCopyNonLazyBinder)((FromBinderGeneric<PresentView>)(object)((MonoInstallerBase)this).Container.Bind<PresentView>()).FromInstance(_viewPrefab)).WhenInjectedInto<PresentView.Factory>();
		((ConditionCopyNonLazyBinder)((FromBinderGeneric<Transform>)(object)((MonoInstallerBase)this).Container.Bind<Transform>()).FromInstance(_viewContainer)).WhenInjectedInto<PresentView.Factory>();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesTo<PresentView.Factory>()).AsSingle();
		((FromBinder)((MonoInstallerBase)this).Container.BindInterfacesTo<PresentView.Manager>()).FromNewComponentOn(_viewContainer.gameObject).AsSingle();
	}
}
