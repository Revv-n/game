using UnityEngine;
using Zenject;

namespace StripClub.UI.Shop;

public class BundleChainLotViewInstaller : MonoInstaller<BundleChainLotViewInstaller>
{
	[SerializeField]
	private BundleChainLotViewFactory viewFactory;

	public override void InstallBindings()
	{
		base.Container.BindInterfacesAndSelfTo<BundleChainLotViewFactory>().FromInstance(viewFactory);
		base.Container.BindInterfacesAndSelfTo<ChainBundleLotView.Manager>().FromNewComponentOn(base.gameObject).AsSingle();
	}
}
