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
		base.Container.BindInterfacesTo<ContainerFactory>().AsSingle().WithArguments(lotViewPrefab, offerContainer);
		base.Container.BindInterfacesTo<LotView.LotContainerManager>().FromNewComponentOn(base.gameObject).AsSingle();
	}
}
