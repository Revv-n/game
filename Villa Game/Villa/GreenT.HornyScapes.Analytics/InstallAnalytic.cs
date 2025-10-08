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
			stream = serverClock.IsReady.Where((bool x) => x).Take(1).Subscribe(delegate
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
		AmplitudeEvent analyticsEvent = new AmplitudeEvent("install");
		amplitude.AddEvent(analyticsEvent);
	}

	public override void Dispose()
	{
		stream?.Dispose();
		base.Dispose();
	}
}
