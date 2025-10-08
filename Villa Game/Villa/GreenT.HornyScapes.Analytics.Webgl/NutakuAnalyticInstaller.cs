using GreenT.HornyScapes.Analytics.Nutaku;
using Zenject;

namespace GreenT.HornyScapes.Analytics.Webgl;

public class NutakuAnalyticInstaller : Installer<NutakuAnalyticInstaller>
{
	public override void InstallBindings()
	{
		BindBuildOnlySystems();
		Bind<GreenT.HornyScapes.Analytics.Nutaku.PlayerPaymentStatsAnalytic>();
		Bind<TutorialAnalytic>();
		BindSingle<CohortAnalyticConverterNutaku>();
	}

	private void BindBuildOnlySystems()
	{
		Bind<MonetizationAnalytic>();
	}

	private void Bind<T>() where T : BaseAnalytic
	{
		base.Container.BindInterfacesAndSelfTo<T>().AsSingle().OnInstantiated(delegate(InjectContext context, T obj)
		{
			base.Container.Resolve<AnalyticSystemManager>().Add(obj);
		})
			.NonLazy();
	}

	private void BindSingle<T>()
	{
		base.Container.BindInterfacesAndSelfTo<T>().AsSingle();
	}
}
