using GreenT.HornyScapes.Bank.GoldenTickets;
using GreenT.HornyScapes.Bank.GoldenTickets.UI;
using Zenject;

namespace GreenT.HornyScapes;

public class RestoreEnergyInstaller : Installer<RestoreEnergyInstaller>
{
	public override void InstallBindings()
	{
		((FromBinderGeneric<EnergyRestore>)(object)((InstallerBase)this).Container.Bind<EnergyRestore>()).FromFactory<EnergyRestoreFactory>().AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<RestoreEnergyPopupOpener>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<RestoreEnergyPopupSetter>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<RestoreEnergyViewController>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<AvailableGoldenTicketFinder>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<GoldenTicketViewController>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<SubscriptionGoldenTicketViewController>()).AsSingle();
	}
}
