using GreenT.HornyScapes.Sellouts.Services;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Sellouts.Windows;

public class SelloutUiInstaller : MonoInstaller
{
	[SerializeField]
	private SelloutWindow _selloutWindow;

	[SerializeField]
	private SelloutRewardsTracker _selloutRewardsTracker;

	public override void InstallBindings()
	{
		base.Container.BindInterfacesAndSelfTo<SelloutWindow>().FromInstance(_selloutWindow).AsSingle();
		base.Container.BindInterfacesAndSelfTo<SelloutRewardsTracker>().FromInstance(_selloutRewardsTracker).AsSingle();
	}
}
