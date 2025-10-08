using System;
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
