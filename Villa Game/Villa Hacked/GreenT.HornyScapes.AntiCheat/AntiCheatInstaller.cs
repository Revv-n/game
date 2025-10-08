using System;
using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.Extensions;
using GreenT.Settings.Data;
using Zenject;

namespace GreenT.HornyScapes.AntiCheat;

public sealed class AntiCheatInstaller : Installer<AntiCheatInstaller>
{
	public override void InstallBindings()
	{
		BindRequests();
	}

	private void BindRequests()
	{
		((InstallerBase)this).Container.BindPostRequest<UpdateCheatingStatusRequest>(PostRequestType.CheaterRequest);
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<CheatEngineSearchService>()).AsSingle();
		Bind<CheaterAnalytic>();
	}

	private void Bind<T>() where T : BaseAnalytic
	{
		((NonLazyBinder)((InstantiateCallbackConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<T>()).AsSingle()).OnInstantiated<T>((Action<InjectContext, T>)delegate(InjectContext context, T obj)
		{
			((InstallerBase)this).Container.Resolve<AnalyticSystemManager>().Add(obj);
		})).NonLazy();
	}
}
