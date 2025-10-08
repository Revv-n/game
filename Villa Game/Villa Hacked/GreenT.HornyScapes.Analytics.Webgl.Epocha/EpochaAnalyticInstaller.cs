using System;
using GreenT.HornyScapes.Analytics.Epocha;
using GreenT.HornyScapes.Extensions;
using GreenT.Settings;
using GreenT.Settings.Data;
using Zenject;

namespace GreenT.HornyScapes.Analytics.Webgl.Epocha;

public class EpochaAnalyticInstaller : Installer<EpochaAnalyticInstaller>
{
	public override void InstallBindings()
	{
		BindBuildOnlySystems();
		Bind<RegistrationAnalytic>();
		Bind<GreenT.HornyScapes.Analytics.Epocha.TutorialAnalytic>();
		Bind<PlayerPaymentStatsAnalytic>();
		BindSingle<CohortAnalyticConverterDefault>();
		BindPartners();
		BindPlayButtonEvent();
		BindPixelEvents();
		Bind<CreateUserAnalytic>();
		Bind<PixelLoadingAnalytic>();
	}

	private void BindBuildOnlySystems()
	{
		Bind<MonetizationAnalytic>();
	}

	private void BindPartners()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.Bind<PartnerEventRequest>()).AsSingle();
		((InstallerBase)this).Container.BindUrlWhenInjectedToType<PartnerEventRequest>(PostRequestType.PartnerEvent);
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<PartnerSender>()).AsSingle();
		((ConditionCopyNonLazyBinder)((FromBinderGeneric<string>)(object)((InstallerBase)this).Container.Bind<string>()).FromResolveGetter<IProjectSettings>((Func<IProjectSettings, string>)((IProjectSettings _settings) => (_settings.RequestUrlResolver as RequestSettings).AppName))).WhenInjectedInto<PartnerSender>();
	}

	private void BindPlayButtonEvent()
	{
		Type typeFromHandle = typeof(PlayButtonEventDummy);
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesTo(typeFromHandle)).AsSingle();
	}

	private void BindPixelEvents()
	{
	}

	private void Editor(DiContainer Container)
	{
		((ArgConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)Container.Bind<IEvent>().WithId((object)"Load").To<EditorLogEvent>()).AsCached()).WithArguments<string>("Add load pixel");
		((ArgConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)Container.Bind<IEvent>().WithId((object)"Payment").To<EditorLogEvent>()).AsCached()).WithArguments<string>("Add Pay pixel");
		((ArgConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)Container.Bind<IEvent>().WithId((object)"SignUp").To<EditorLogEvent>()).AsCached()).WithArguments<string>("Add SignUp pixel");
		((ArgConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)Container.Bind<IEvent>().WithId((object)"Tutorial").To<EditorLogEvent>()).AsCached()).WithArguments<string>("Add TutorialEnd pixel");
		Container.BindUrlWhenInjectedToType<EditorUrlReader>(PostRequestType.WebGLReader);
		BindSingle<EditorUrlReader>();
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
