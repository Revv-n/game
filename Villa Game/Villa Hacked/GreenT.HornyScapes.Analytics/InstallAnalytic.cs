using System;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Analytics;

public class InstallAnalytic : BaseAnalytic
{
	private readonly IClock clock;

	private const string ANALYTIC_EVENT = "install";

	private IDisposable stream;

	public InstallAnalytic(IAmplitudeSender<AmplitudeEvent> amplitude, IClock clock)
		: base(amplitude)
	{
		this.clock = clock;
		if (clock is ServerClock serverClock)
		{
			stream = ObservableExtensions.Subscribe<bool>(Observable.Take<bool>(Observable.Where<bool>((IObservable<bool>)serverClock.IsReady, (Func<bool, bool>)((bool x) => x)), 1), (Action<bool>)delegate
			{
				Check();
			});
		}
		else
		{
			Check();
		}
	}

	private void Check()
	{
		if (PlayerPrefs.GetInt("install", 0) == 0)
		{
			SendEvent();
			PlayerPrefs.SetInt("install", 1);
			PlayerPrefs.Save();
		}
	}

	private void SendEvent()
	{
		AmplitudeEvent amplitudeEvent = new AmplitudeEvent("install");
		((IAnalyticSender<AmplitudeEvent>)(object)amplitude).AddEvent(amplitudeEvent);
	}

	public override void Dispose()
	{
		stream?.Dispose();
		base.Dispose();
	}
}
