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
		((ConditionCopyNonLazyBinder)((FromBinderGeneric<Transform>)(object)((MonoInstallerBase)this).Container.Bind<Transform>()).FromInstance(optionsContainer)).WhenInjectedInto<ResponseOptionView.Factory>();
		((ConditionCopyNonLazyBinder)((FromBinderGeneric<ResponseOptionView>)(object)((MonoInstallerBase)this).Container.Bind<ResponseOptionView>()).FromInstance(optionViewPrefab)).WhenInjectedInto<ResponseOptionView.Factory>();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesTo<ResponseOptionView.Factory>()).AsCached();
		((FromBinder)((MonoInstallerBase)this).Container.Bind<ResponseOptionView.Manager>()).FromNewComponentSibling().AsSingle();
	}
}
