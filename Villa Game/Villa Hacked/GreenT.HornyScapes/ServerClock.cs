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
			observableApplicationPause = Observable.Share<bool>(Observable.SkipWhile<bool>(MainThreadDispatcher.OnApplicationPauseAsObservable(), (Func<bool, bool>)((bool x) => !x)));
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
		serverRequestStream = ObservableExtensions.Subscribe<DateTime>(Observable.Select<Response<long>, DateTime>(Observable.Catch<Response<long>, UnityWebRequestException>(Observable.Merge<Response<long>>(Observable.SelectMany<bool, Response<long>>(Observable.Where<bool>(observableApplicationPause, (Func<bool, bool>)((bool x) => !x)), (Func<bool, IObservable<Response<long>>>)((bool _) => request)), new IObservable<Response<long>>[1] { request }), (Func<UnityWebRequestException, IObservable<Response<long>>>)delegate(UnityWebRequestException ex)
		{
			error = true;
			errorMessage = ex.Error;
			throw ex.SendException("Error on server time request: " + errorMessage);
		}), (Func<Response<long>, DateTime>)((Response<long> _response) => TimeExtension.ConvertFromUnixTimestamp(_response.Data))), (Action<DateTime>)SetTime);
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
