using Zenject;

namespace GreenT.HornyScapes.Presents.Analytics;

public class AnalyticInstaller : Installer<AnalyticInstaller>
{
	public override void InstallBindings()
	{
		base.Container.Bind<PresentsAnalytic>().AsSingle();
	}
}
