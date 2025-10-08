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
		((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<SelloutWindow>().FromInstance((object)_selloutWindow).AsSingle();
		((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<SelloutRewardsTracker>().FromInstance((object)_selloutRewardsTracker).AsSingle();
	}
}
