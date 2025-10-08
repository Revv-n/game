using Zenject;

namespace GreenT.HornyScapes.Cheats;

public class RewindTimeCheatsInstaller : Installer<RewindTimeCheatsInstaller>
{
	public override void InstallBindings()
	{
		base.Container.Bind<TimeRewinder>().FromNew().AsSingle();
	}
}
