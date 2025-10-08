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
		base.Container.BindInterfacesAndSelfTo<MiniEventShopChainBundlesViewFactory>().FromInstance(_viewFactory);
		base.Container.BindInterfacesAndSelfTo<MiniEventShopChainBundlesViewManager>().FromInstance(_viewManager);
	}
}
