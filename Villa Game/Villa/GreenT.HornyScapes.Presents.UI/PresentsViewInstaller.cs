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
		base.Container.Bind<PresentView>().FromInstance(_viewPrefab).WhenInjectedInto<PresentView.Factory>();
		base.Container.Bind<Transform>().FromInstance(_viewContainer).WhenInjectedInto<PresentView.Factory>();
		base.Container.BindInterfacesTo<PresentView.Factory>().AsSingle();
		base.Container.BindInterfacesTo<PresentView.Manager>().FromNewComponentOn(_viewContainer.gameObject).AsSingle();
	}
}
