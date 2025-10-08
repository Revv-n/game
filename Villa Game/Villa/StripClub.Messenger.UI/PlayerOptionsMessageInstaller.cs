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
		base.Container.BindIFactory<ResponseOption, ResponseOptionView>().FromComponentInNewPrefab(optionViewPrefab).UnderTransform(optionsContainer)
			.AsCached();
		base.Container.Bind<ResponseOptionView.Manager>().FromNewComponentSibling().AsSingle();
	}
}
