using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Messenger.UI;

public class PlayerOptionsMessageInstaller : MonoInstaller<PlayerOptionsMessageInstaller>
{
	[SerializeField]
	private Transform optionsContainer;

	[SerializeField]
	private ResponseOptionView optionViewPrefab;

	public override void InstallBindings()
	{
		base.Container.Bind<Transform>().FromInstance(optionsContainer).WhenInjectedInto<ResponseOptionView.Factory>();
		base.Container.Bind<ResponseOptionView>().FromInstance(optionViewPrefab).WhenInjectedInto<ResponseOptionView.Factory>();
		base.Container.BindInterfacesTo<ResponseOptionView.Factory>().AsCached();
		base.Container.Bind<ResponseOptionView.Manager>().FromNewComponentSibling().AsSingle();
	}
}
