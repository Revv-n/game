using Zenject;

namespace GreenT.HornyScapes.Cheats;

public class RewindTimeCheatsInstaller : Installer<RewindTimeCheatsInstaller>
{
	public override void InstallBindings()
	{
		((FromBinder)((InstallerBase)this).Container.Bind<TimeRewinder>()).FromNew().AsSingle();
	}
}
