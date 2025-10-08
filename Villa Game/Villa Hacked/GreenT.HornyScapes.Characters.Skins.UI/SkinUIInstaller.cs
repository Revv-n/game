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
		((ConditionCopyNonLazyBinder)((FromBinderGeneric<PromoteSkinView>)(object)((MonoInstallerBase)this).Container.Bind<PromoteSkinView>()).FromInstance(viewPrefab)).WhenInjectedInto<PromoteSkinView.Factory>();
		((ConditionCopyNonLazyBinder)((FromBinderGeneric<Transform>)(object)((MonoInstallerBase)this).Container.Bind<Transform>()).FromInstance(skinViewContainer)).WhenInjectedInto<PromoteSkinView.Factory>();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesTo<PromoteSkinView.Factory>()).AsCached();
		((FromBinder)((MonoInstallerBase)this).Container.BindInterfacesTo<PromoteSkinView.Manager>()).FromNewComponentOn(skinViewContainer.gameObject).AsSingle();
	}
}
