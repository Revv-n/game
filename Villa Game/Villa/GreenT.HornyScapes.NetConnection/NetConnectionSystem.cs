using System;
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

	public IObservable<Exception> OnError => onError.AsObservable();

	public IObservable<string> OnSuccess => onSuccess.AsObservable();

	public NetConnectionSystem(NetConnectionRequest netConnectionRequest, int intervalTime)
	{
		this.netConnectionRequest = netConnectionRequest;
		this.intervalTime = TimeSpan.FromSeconds(intervalTime);
	}

	public void ManualCheckConnection()
	{
		IObservable<long> initialObservable = Observable.Return(0L);
		IObservable<string> observable = CreatePingObservable(initialObservable).DoOnError(delegate
		{
			IsPinging.Value = false;
		});
		CheckConnection(observable);
	}

	public void Initialize()
	{
		IObservable<long> initialObservable = Observable.Interval(intervalTime, Scheduler.MainThreadIgnoreTimeScale).Repeat();
		IObservable<string> observable = CreatePingObservable(initialObservable).Catch(delegate(Exception ex)
		{
			throw ex.LogException();
		}).OnErrorRetry(delegate(Exception ex)
		{
			IsPinging.Value = false;
			onError.OnNext(ex);
		});
		CheckConnection(observable);
	}

	private IObservable<string> CreatePingObservable(IObservable<long> initialObservable)
	{
		return initialObservable.Where((long _) => !IsPinging.Value).Do(delegate
		{
			IsPinging.Value = true;
		}).SelectMany((Func<long, IObservable<string>>)Ping)
			.Do(delegate
			{
				IsPinging.Value = false;
			});
	}

	private void CheckConnection(IObservable<string> observable)
	{
		observable.Subscribe(onSuccess.OnNext).AddTo(streams);
	}

	private IObservable<string> Ping(long t)
	{
		return netConnectionRequest.Ping().Take(1);
	}

	public void Dispose()
	{
		onError.Dispose();
		onSuccess.Dispose();
		IsPinging.Dispose();
		streams.Dispose();
	}
}
