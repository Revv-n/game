using System;
using GreenT.HornyScapes.Analytics.Harem;
using Zenject;

namespace GreenT.HornyScapes.Analytics.Webgl;

public class HaremAnalyticInstaller : Installer<HaremAnalyticInstaller>
{
	public override void InstallBindings()
	{
		BindBuildOnlySystems();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((ConcreteBinderGeneric<IMarketingEventSender>)(object)((InstallerBase)this).Container.Bind<IMarketingEventSender>()).To<MarketingEventSender>()).AsSingle();
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
		((NonLazyBinder)((InstantiateCallbackConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<T>()).AsSingle()).OnInstantiated<T>((Action<InjectContext, T>)delegate(InjectContext context, T obj)
		{
			((InstallerBase)this).Container.Resolve<AnalyticSystemManager>().Add(obj);
		})).NonLazy();
	}

	private void BindSingle<T>()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<T>()).AsSingle();
	}
}
