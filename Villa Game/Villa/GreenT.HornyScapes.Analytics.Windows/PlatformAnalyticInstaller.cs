using GreenT.HornyScapes.Analytics.Steam;
using Zenject;

namespace GreenT.HornyScapes.Analytics.Windows;

public class PlatformAnalyticInstaller : Installer<PlatformAnalyticInstaller>
{
	public override void InstallBindings()
	{
		Bind<PlayerPaymentStatsAnalytic>();
		BindSingle<CohortAnalyticConverterDefault>();
		Bind<TutorialAnalytic>();
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
