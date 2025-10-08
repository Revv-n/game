using Zenject;

namespace GreenT.HornyScapes.Presents.Analytics;

public class AnalyticInstaller : Installer<AnalyticInstaller>
{
	public override void InstallBindings()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.Bind<PresentsAnalytic>()).AsSingle();
	}
}
