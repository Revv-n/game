using GreenT.HornyScapes.Extensions;
using GreenT.Settings.Data;
using Zenject;

namespace GreenT.HornyScapes.Analytics;

public class AnalyticSendersInstaller : Installer<AnalyticSendersInstaller>
{
	public override void InstallBindings()
	{
		BindAmplitude();
	}

	private void BindAmplitude()
	{
		base.Container.BindInterfacesAndSelfTo<Amplitude>().AsSingle().WithArguments("73ba9c5a897be31381ca0af9f1189dad");
		base.Container.BindUrlWhenInjectedToType<Amplitude>(PostRequestType.AmplitudeEventServerMirror);
		base.Container.BindInterfacesAndSelfTo<AnalyticUserProperties>().AsSingle();
	}
}
