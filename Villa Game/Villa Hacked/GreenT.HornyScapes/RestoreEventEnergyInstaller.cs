using Zenject;

namespace GreenT.HornyScapes;

public class RestoreEventEnergyInstaller : Installer<RestoreEventEnergyInstaller>
{
	public override void InstallBindings()
	{
		((FromBinderGeneric<RestoreEventEnergy>)(object)((InstallerBase)this).Container.Bind<RestoreEventEnergy>()).FromFactory<RestoreEventEnergyFactory>().AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<RestoreEventEnergyPopupOpener>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<RestoreEventEnergyViewController>()).AsSingle();
	}
}
