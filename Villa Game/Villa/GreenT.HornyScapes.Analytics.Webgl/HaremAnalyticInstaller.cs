using GreenT.HornyScapes.Analytics.Harem;
using Zenject;

namespace GreenT.HornyScapes.Analytics.Webgl;

public class HaremAnalyticInstaller : Installer<HaremAnalyticInstaller>
{
	public override void InstallBindings()
	{
		BindBuildOnlySystems();
		base.Container.Bind<IMarketingEventSender>().To<MarketingEventSender>().AsSingle();
		Bind<RegistrationAnalytic>();
		Bind<GreenT.HornyScapes.Analytics.Harem.TutorialAnalytic>();
		Bind<PlayerPaymentStatsAnalytic>();
		BindSingle<CohortAnalyticConverterDefault>();
		Bind<PlayButtonMarketingAnalytic>();
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
