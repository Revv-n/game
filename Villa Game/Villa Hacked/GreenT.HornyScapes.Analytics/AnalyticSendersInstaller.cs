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
		((ArgConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<Amplitude>()).AsSingle()).WithArguments<string>("73ba9c5a897be31381ca0af9f1189dad");
		((InstallerBase)this).Container.BindUrlWhenInjectedToType<Amplitude>(PostRequestType.AmplitudeEventServerMirror);
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<AnalyticUserProperties>()).AsSingle();
	}
}
