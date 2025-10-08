using UnityEngine;
using Zenject;

namespace StripClub.Messenger.UI;

public class PlayerOptionsMessageInstaller : MonoInstaller<PlayerOptionsMessageInstaller>
{
	[SerializeField]
	private Transform optionsContainer;

	[SerializeField]
	private ResponseOptionView optionViewPrefab;

	public override void InstallBindings()
	{
		((TransformScopeConcreteIdArgConditionCopyNonLazyBinder)((FactoryFromBinderBase)((MonoInstallerBase)this).Container.BindIFactory<ResponseOption, ResponseOptionView>()).FromComponentInNewPrefab((Object)optionViewPrefab)).UnderTransform(optionsContainer).AsCached();
		((FromBinder)((MonoInstallerBase)this).Container.Bind<ResponseOptionView.Manager>()).FromNewComponentSibling().AsSingle();
	}
}
