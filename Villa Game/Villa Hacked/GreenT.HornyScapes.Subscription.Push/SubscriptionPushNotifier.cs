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
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		_forcePushSubject.OnNext(default(Unit));
	}

	protected void StartTimer(TimeSpan pushTime)
	{
		Timer.Start(pushTime);
	}

	public IObservable<Unit> OnPushRequest()
	{
		return Observable.Merge<Unit>(Observable.AsUnitObservable<GenericTimer>(Timer.OnTimeIsUp), new IObservable<Unit>[1] { Observable.AsObservable<Unit>((IObservable<Unit>)_forcePushSubject) });
	}

	public void Dispose()
	{
		CompositeDisposable compositeDisposable = CompositeDisposable;
		if (compositeDisposable != null)
		{
			compositeDisposable.Dispose();
		}
	}
}
