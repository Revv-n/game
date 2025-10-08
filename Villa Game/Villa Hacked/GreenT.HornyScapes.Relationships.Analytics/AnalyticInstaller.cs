using Zenject;

namespace GreenT.HornyScapes.Relationships.Analytics;

public class AnalyticInstaller : Installer<AnalyticInstaller>
{
	public override void InstallBindings()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.Bind<RelationshipAnalytic>()).AsSingle();
	}
}
