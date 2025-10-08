using GreenT.HornyScapes.Analytics.Erolabs;
using Zenject;

namespace GreenT.HornyScapes.Analytics.Android;

public class ErolabsAnalyticInstaller : Installer<ErolabsAnalyticInstaller>
{
	public override void InstallBindings()
	{
		Bind<PlayerPaymentStatsAnalytic>();
		Bind<MonetizationAnalytic>();
		Bind<TutorialAnalytic>();
		BindSingle<CohortAnalyticConverterDefault>();
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
