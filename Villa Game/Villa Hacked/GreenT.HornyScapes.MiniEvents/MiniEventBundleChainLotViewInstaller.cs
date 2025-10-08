using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class MiniEventBundleChainLotViewInstaller : MonoInstaller<MiniEventBundleChainLotViewInstaller>
{
	[SerializeField]
	private MiniEventShopChainBundlesViewFactory _viewFactory;

	[SerializeField]
	private MiniEventShopChainBundlesViewManager _viewManager;

	public override void InstallBindings()
	{
		((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventShopChainBundlesViewFactory>().FromInstance((object)_viewFactory);
		((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventShopChainBundlesViewManager>().FromInstance((object)_viewManager);
	}
}
