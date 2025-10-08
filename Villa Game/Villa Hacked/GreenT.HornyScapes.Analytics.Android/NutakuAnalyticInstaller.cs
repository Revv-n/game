using System;
using GreenT.HornyScapes.Analytics.Nutaku;
using Zenject;

namespace GreenT.HornyScapes.Analytics.Android;

public class NutakuAnalyticInstaller : Installer<NutakuAnalyticInstaller>
{
	public override void InstallBindings()
	{
		Bind<GreenT.HornyScapes.Analytics.Nutaku.PlayerPaymentStatsAnalytic>();
		Bind<MonetizationAnalytic>();
		Bind<TutorialAnalytic>();
		BindSingle<CohortAnalyticConverterNutaku>();
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
