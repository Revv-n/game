using GreenT.HornyScapes.Lockers;
using Zenject;

namespace StripClub.Model;

public class LockerInstaller : Installer<LockerInstaller>
{
	public override void InstallBindings()
	{
		base.Container.BindInterfacesAndSelfTo<LockerController>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<LockerManager>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<LockerFactory>().AsCached();
	}
}
