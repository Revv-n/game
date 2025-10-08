using UnityEngine;
using Zenject;

namespace StripClub.UI.Shop;

public class BundleChainLotViewInstaller : MonoInstaller<BundleChainLotViewInstaller>
{
	[SerializeField]
	private BundleChainLotViewFactory viewFactory;

	public override void InstallBindings()
	{
		((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<BundleChainLotViewFactory>().FromInstance((object)viewFactory);
		((FromBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<ChainBundleLotView.Manager>()).FromNewComponentOn(((Component)(object)this).gameObject).AsSingle();
	}
}
