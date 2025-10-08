using Zenject;

namespace GreenT.HornyScapes.Relationships.Analytics;

public class AnalyticInstaller : Installer<AnalyticInstaller>
{
	public override void InstallBindings()
	{
		base.Container.Bind<RelationshipAnalytic>().AsSingle();
	}
}
