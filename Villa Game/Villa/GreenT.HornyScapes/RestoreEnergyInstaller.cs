using GreenT.HornyScapes.Bank.GoldenTickets;
using GreenT.HornyScapes.Bank.GoldenTickets.UI;
using Zenject;

namespace GreenT.HornyScapes;

public class RestoreEnergyInstaller : Installer<RestoreEnergyInstaller>
{
	public override void InstallBindings()
	{
		base.Container.Bind<EnergyRestore>().FromFactory<EnergyRestoreFactory>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<RestoreEnergyPopupOpener>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<RestoreEnergyPopupSetter>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<RestoreEnergyViewController>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<AvailableGoldenTicketFinder>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<GoldenTicketViewController>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<SubscriptionGoldenTicketViewController>().AsSingle();
	}
}
