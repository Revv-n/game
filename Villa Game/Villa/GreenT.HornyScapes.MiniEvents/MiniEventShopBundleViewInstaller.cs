using GreenT.HornyScapes.Extensions;
using StripClub.Model.Shop;
using StripClub.UI.Shop;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class MiniEventShopBundleViewInstaller : MonoInstaller
{
	[SerializeField]
	private SimpleBundleLotView _simpleBundleLotView;

	[SerializeField]
	private MiniEventShopBundleViewManager _miniEventShopBundleViewManager;

	[SerializeField]
	private Transform _root;

	public override void InstallBindings()
	{
		base.Container.BindInterfacesAndSelfTo<MiniEventShopBundleViewManager>().FromInstance(_miniEventShopBundleViewManager).AsSingle();
		base.Container.BindViewFactory<Lot, SimpleBundleLotView>(_root, _simpleBundleLotView);
	}
}
