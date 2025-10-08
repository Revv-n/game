using System;
using System.Collections.Generic;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.NetConnection;

public class NetConnectionSystem : IInitializable, IDisposable
{
	private readonly Subject<Exception> onError = new Subject<Exception>();

	private readonly Subject<string> onSuccess = new Subject<string>();

	public readonly ReactiveProperty<bool> IsPinging = new ReactiveProperty<bool>();

	private readonly NetConnectionRequest netConnectionRequest;

	private readonly TimeSpan intervalTime;

	private readonly CompositeDisposable streams = new CompositeDisposable();

	public IObservable<Exception> OnError => Observable.AsObservable<Exception>((IObservable<Exception>)onError);

	public IObservable<string> OnSuccess => Observable.AsObservable<string>((IObservable<string>)onSuccess);

	public NetConnectionSystem(NetConnectionRequest netConnectionRequest, int intervalTime)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		this.netConnectionRequest = netConnectionRequest;
		this.intervalTime = TimeSpan.FromSeconds(intervalTime);
	}

	public void ManualCheckConnection()
	{
		IObservable<long> initialObservable = Observable.Return<long>(0L);
		IObservable<string> observable = Observable.DoOnError<string>(CreatePingObservable(initialObservable), (Action<Exception>)delegate
		{
			IsPinging.Value = false;
		});
		CheckConnection(observable);
	}

	public void Initialize()
	{
		IObservable<long> initialObservable = Observable.Repeat<long>(Observable.Interval(intervalTime, Scheduler.MainThreadIgnoreTimeScale));
		IObservable<string> observable = Observable.OnErrorRetry<string, Exception>(Observable.Catch<string, Exception>(CreatePingObservable(initialObservable), (Func<Exception, IObservable<string>>)delegate(Exception ex)
		{
			throw ex.LogException();
		}), (Action<Exception>)delegate(Exception ex)
		{
			IsPinging.Value = false;
			onError.OnNext(ex);
		});
		CheckConnection(observable);
	}

	private IObservable<string> CreatePingObservable(IObservable<long> initialObservable)
	{
		return Observable.Do<string>(Observable.SelectMany<long, string>(Observable.Do<long>(Observable.Where<long>(initialObservable, (Func<long, bool>)((long _) => !IsPinging.Value)), (Action<long>)delegate
		{
			IsPinging.Value = true;
		}), (Func<long, IObservable<string>>)Ping), (Action<string>)delegate
		{
			IsPinging.Value = false;
		});
	}

	private void CheckConnection(IObservable<string> observable)
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<string>(observable, (Action<string>)onSuccess.OnNext), (ICollection<IDisposable>)streams);
	}

	private IObservable<string> Ping(long t)
	{
		return Observable.Take<string>(netConnectionRequest.Ping(), 1);
	}

	public void Dispose()
	{
		onError.Dispose();
		onSuccess.Dispose();
		IsPinging.Dispose();
		streams.Dispose();
	}
}
