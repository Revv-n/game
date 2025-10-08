using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Characters.Skins.UI;

public class SkinUIInstaller : MonoInstaller<SkinUIInstaller>
{
	[SerializeField]
	private PromoteSkinView viewPrefab;

	[SerializeField]
	private Transform skinViewContainer;

	public override void InstallBindings()
	{
		base.Container.Bind<PromoteSkinView>().FromInstance(viewPrefab).WhenInjectedInto<PromoteSkinView.Factory>();
		base.Container.Bind<Transform>().FromInstance(skinViewContainer).WhenInjectedInto<PromoteSkinView.Factory>();
		base.Container.BindInterfacesTo<PromoteSkinView.Factory>().AsCached();
		base.Container.BindInterfacesTo<PromoteSkinView.Manager>().FromNewComponentOn(skinViewContainer.gameObject).AsSingle();
	}
}
