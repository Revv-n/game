using GreenT.HornyScapes.Events;
using Zenject;

namespace GreenT.HornyScapes;

public sealed class LastChanceInstaller : MonoInstaller<LastChanceInstaller>
{
	public override void InstallBindings()
	{
		BindStrategies();
		base.Container.BindInterfacesAndSelfTo<LastChanceEventBundleProvider>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<LastChanceController>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<LastChanceInitializer>().AsSingle();
	}

	private void BindStrategies()
	{
		base.Container.BindInterfacesAndSelfTo<LastChanceRatingsStrategy>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<LastChanceEventBattlePassStrategy>().AsSingle();
	}
}
