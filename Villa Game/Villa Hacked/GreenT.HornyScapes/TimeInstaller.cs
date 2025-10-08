using System;
using GreenT.Model.Collections;
using GreenT.Settings;
using GreenT.Settings.Data;
using StripClub.Extensions;
using StripClub.UI;
using Zenject;

namespace GreenT.HornyScapes;

public class TimeInstaller : Installer<TimeInstaller>
{
	public class TimerCollection : SimpleManager<GenericTimer>
	{
	}

	public override void InstallBindings()
	{
		BindTimer();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<TimerCollection>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.Bind<TimeHelper>()).AsSingle();
	}

	private void BindTimer()
	{
		LocalTime();
	}

	private void LocalTime()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesTo<LocalClock>()).AsSingle();
	}

	private void ServerTime()
	{
		((ConditionCopyNonLazyBinder)((FromBinderGeneric<string>)(object)((InstallerBase)this).Container.Bind<string>()).FromResolveGetter<IProjectSettings>((Func<IProjectSettings, string>)GetTimeRequestUrl)).WhenInjectedInto<ServerClock>();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesTo<ServerClock>()).AsSingle();
	}

	private static string GetTimeRequestUrl(IProjectSettings projectSettings)
	{
		PostRequestType type = PostRequestType.GetTime;
		return projectSettings.RequestUrlResolver.PostRequestUrl(type);
	}
}
