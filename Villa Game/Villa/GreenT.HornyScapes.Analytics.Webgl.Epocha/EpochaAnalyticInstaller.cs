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
		base.Container.Bind<PartnerEventRequest>().AsSingle();
		base.Container.BindUrlWhenInjectedToType<PartnerEventRequest>(PostRequestType.PartnerEvent);
		base.Container.BindInterfacesAndSelfTo<PartnerSender>().AsSingle();
		base.Container.Bind<string>().FromResolveGetter((IProjectSettings _settings) => (_settings.RequestUrlResolver as RequestSettings).AppName).WhenInjectedInto<PartnerSender>();
	}

	private void BindPlayButtonEvent()
	{
		Type typeFromHandle = typeof(PlayButtonEventDummy);
		base.Container.BindInterfacesTo(typeFromHandle).AsSingle();
	}

	private void BindPixelEvents()
	{
	}

	private void Editor(DiContainer Container)
	{
		Container.Bind<IEvent>().WithId("Load").To<EditorLogEvent>()
			.AsCached()
			.WithArguments("Add load pixel");
		Container.Bind<IEvent>().WithId("Payment").To<EditorLogEvent>()
			.AsCached()
			.WithArguments("Add Pay pixel");
		Container.Bind<IEvent>().WithId("SignUp").To<EditorLogEvent>()
			.AsCached()
			.WithArguments("Add SignUp pixel");
		Container.Bind<IEvent>().WithId("Tutorial").To<EditorLogEvent>()
			.AsCached()
			.WithArguments("Add TutorialEnd pixel");
		Container.BindUrlWhenInjectedToType<EditorUrlReader>(PostRequestType.WebGLReader);
		BindSingle<EditorUrlReader>();
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
