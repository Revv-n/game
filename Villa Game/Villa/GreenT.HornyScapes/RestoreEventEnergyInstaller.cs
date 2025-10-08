using Zenject;

namespace GreenT.HornyScapes;

public class RestoreEventEnergyInstaller : Installer<RestoreEventEnergyInstaller>
{
	public override void InstallBindings()
	{
		base.Container.Bind<RestoreEventEnergy>().FromFactory<RestoreEventEnergyFactory>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<RestoreEventEnergyPopupOpener>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<RestoreEventEnergyViewController>().AsSingle();
	}
}
