using GreenT.HornyScapes.Extensions;
using StripClub.Model.Shop;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class MiniEventShop8SlotsViewInstaller : MonoInstaller
{
	[SerializeField]
	private MiniEvent8SlotsLotView _miniEvent8SlotsLotView;

	[SerializeField]
	private MiniEventShop8SlotsViewManager _miniEventShop8SlotsViewManager;

	[SerializeField]
	private Transform _root;

	public override void InstallBindings()
	{
		base.Container.BindInterfacesAndSelfTo<MiniEventShop8SlotsViewManager>().FromInstance(_miniEventShop8SlotsViewManager).AsSingle();
		base.Container.BindViewFactory<Lot, MiniEvent8SlotsLotView>(_root, _miniEvent8SlotsLotView);
	}
}
