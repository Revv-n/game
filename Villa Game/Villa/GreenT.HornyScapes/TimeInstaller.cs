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
		base.Container.BindInterfacesAndSelfTo<TimerCollection>().AsSingle();
		base.Container.Bind<TimeHelper>().AsSingle();
	}

	private void BindTimer()
	{
		LocalTime();
	}

	private void LocalTime()
	{
		base.Container.BindInterfacesTo<LocalClock>().AsSingle();
	}

	private void ServerTime()
	{
		base.Container.Bind<string>().FromResolveGetter<IProjectSettings>(GetTimeRequestUrl).WhenInjectedInto<ServerClock>();
		base.Container.BindInterfacesTo<ServerClock>().AsSingle();
	}

	private static string GetTimeRequestUrl(IProjectSettings projectSettings)
	{
		PostRequestType type = PostRequestType.GetTime;
		return projectSettings.RequestUrlResolver.PostRequestUrl(type);
	}
}
