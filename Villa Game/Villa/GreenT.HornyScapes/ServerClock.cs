using System;
using Cysharp.Threading.Tasks;
using GreenT.Net;
using StripClub.Extensions;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes;

public class ServerClock : IClock, ITimeRewinder, IDisposable
{
	private static IObservable<bool> observableApplicationPause;

	public ReactiveProperty<bool> IsReady = new ReactiveProperty<bool>();

	protected DateTime initialTime;

	protected DateTime initialTimeLocal;

	protected TimeSpan timePast;

	private IDisposable timerStream;

	private IDisposable serverRequestStream;

	private readonly string url;

	private bool error = true;

	private string errorMessage;

	private float startTime;

	private TimeSpan skipTime = TimeSpan.Zero;

	static ServerClock()
	{
		if (Application.isPlaying)
		{
			observableApplicationPause = MainThreadDispatcher.OnApplicationPauseAsObservable().SkipWhile((bool x) => !x).Share();
		}
	}

	public ServerClock(string url)
	{
		this.url = url;
		startTime = Time.realtimeSinceStartup;
		initialTimeLocal = DateTime.UtcNow;
		SynchronizeTime();
		errorMessage = "Didn't get server time yet";
	}

	private void SynchronizeTime()
	{
		serverRequestStream?.Dispose();
		IObservable<Response<long>> request = HttpRequestExecutor.GetRequest<Response<long>>(url);
		serverRequestStream = (from _response in observableApplicationPause.Where((bool x) => !x).SelectMany((bool _) => request).Merge(request)
				.Catch(delegate(UnityWebRequestException ex)
				{
					error = true;
					errorMessage = ex.Error;
					throw ex.SendException("Error on server time request: " + errorMessage);
				})
			select TimeExtension.ConvertFromUnixTimestamp(_response.Data)).Subscribe(SetTime);
	}

	public DateTime GetTime()
	{
		timePast = TimeSpan.FromSeconds(Time.realtimeSinceStartup - startTime);
		DateTime utcNow = DateTime.UtcNow;
		if (error)
		{
			new Exception("Get time last try returns the error: " + errorMessage).LogException();
			utcNow = initialTimeLocal + timePast;
		}
		else
		{
			utcNow = initialTime + timePast;
		}
		IsReady.Value = true;
		return utcNow;
	}

	public DateTime GetDate()
	{
		throw new NotImplementedException();
	}

	public TimeSpan GetTimeEndDay()
	{
		throw new NotImplementedException();
	}

	private void SetTime(DateTime dateTime)
	{
		error = false;
		initialTime = dateTime + skipTime;
	}

	public void Dispose()
	{
		timerStream?.Dispose();
		serverRequestStream?.Dispose();
	}

	void ITimeRewinder.Rewind(TimeSpan time)
	{
		skipTime += time;
		initialTime += time;
	}

	public void Reset()
	{
		initialTime -= skipTime;
		skipTime = TimeSpan.Zero;
	}
}
