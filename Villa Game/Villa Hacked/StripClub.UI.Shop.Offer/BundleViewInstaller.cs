using UnityEngine;
using Zenject;

namespace StripClub.UI.Shop.Offer;

public class BundleViewInstaller : MonoInstaller<LotViewInstaller>
{
	[SerializeField]
	private ContainerView lotViewPrefab;

	[SerializeField]
	private Transform offerContainer;

	public override void InstallBindings()
	{
		((ArgConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesTo<ContainerFactory>()).AsSingle()).WithArguments<ContainerView, Transform>(lotViewPrefab, offerContainer);
		((FromBinder)((MonoInstallerBase)this).Container.BindInterfacesTo<LotView.LotContainerManager>()).FromNewComponentOn(((Component)(object)this).gameObject).AsSingle();
	}
}
