using System;
using StripClub.UI;
using UniRx;

namespace GreenT.HornyScapes.Subscription.Push;

[Serializable]
public abstract class SubscriptionPushNotifier : IDisposable
{
	protected readonly CompositeDisposable CompositeDisposable = new CompositeDisposable();

	protected readonly GenericTimer Timer = new GenericTimer(TimeSpan.Zero);

	private readonly Subject<Unit> _forcePushSubject = new Subject<Unit>();

	public virtual void Set()
	{
		_forcePushSubject.OnNext(default(Unit));
	}

	protected void StartTimer(TimeSpan pushTime)
	{
		Timer.Start(pushTime);
	}

	public IObservable<Unit> OnPushRequest()
	{
		return Timer.OnTimeIsUp.AsUnitObservable().Merge(_forcePushSubject.AsObservable());
	}

	public void Dispose()
	{
		CompositeDisposable?.Dispose();
	}
}
