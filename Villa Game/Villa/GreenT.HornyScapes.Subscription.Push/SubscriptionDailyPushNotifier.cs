using System;
using System.Linq;
using GreenT.Data;
using StripClub.Model.Shop;
using StripClub.UI;
using UniRx;

namespace GreenT.HornyScapes.Subscription.Push;

[Serializable]
[MementoHolder]
public class SubscriptionDailyPushNotifier : ISavableState, IDisposable
{
	[Serializable]
	public class Memento : GreenT.Data.Memento
	{
		public TimeSpan TimeUntilRecharge { get; }

		public DateTime SaveTime { get; }

		public Memento(SubscriptionDailyPushNotifier notifier)
			: base(notifier)
		{
			TimeSpan timeSpan = ((notifier.Timer.TimeLeft == TimeSpan.Zero) ? TimeSpan.FromDays(1.0) : notifier.Timer.TimeLeft);
			SaveTime = notifier.GetTime();
			TimeUntilRecharge = timeSpan;
		}
	}

	public readonly SubscriptionLot Lot;

	public readonly SubscriptionModel Model;

	public readonly GenericTimer Timer = new GenericTimer();

	private readonly IClock _clock;

	private readonly Subject<Unit> _onClaimSubject = new Subject<Unit>();

	private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

	private static TimeSpan RechargeTime => TimeSpan.FromDays(1.0);

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public SubscriptionDailyPushNotifier(SubscriptionModel model, LotManager lotManager, IClock clock)
	{
		Model = model;
		Timer.TimeLeft = TimeSpan.Zero;
		Lot = lotManager.Collection.First((Lot lot) => lot is SubscriptionLot && lot.ID == Model.BaseID) as SubscriptionLot;
		_clock = clock;
	}

	public void Set()
	{
		_onClaimSubject.Subscribe(delegate
		{
			StartTimer(RechargeTime);
		}).AddTo(_compositeDisposable);
	}

	private void StartTimer(TimeSpan pushTime)
	{
		Timer.Start(pushTime);
	}

	public IObservable<Unit> OnPushRequest(bool force = false)
	{
		if (force && Timer.TimeLeft == TimeSpan.Zero)
		{
			return Observable.Return(default(Unit));
		}
		return Timer.OnTimeIsUp.AsUnitObservable();
	}

	public void SetClaimed()
	{
		_onClaimSubject.OnNext(default(Unit));
	}

	public DateTime GetTime()
	{
		return _clock.GetTime();
	}

	public void Dispose()
	{
		_compositeDisposable?.Dispose();
	}

	public string UniqueKey()
	{
		return $"subscription_daily{Model.BaseID}";
	}

	public GreenT.Data.Memento SaveState()
	{
		return new Memento(this);
	}

	public void LoadState(GreenT.Data.Memento memento)
	{
		if (memento is Memento { TimeUntilRecharge: var timeSpan } memento2)
		{
			TimeSpan timeSpan2 = _clock.GetTime() - memento2.SaveTime;
			if (timeSpan2.Ticks > 0)
			{
				timeSpan -= timeSpan2;
			}
			if (Timer.TimeLeft.Ticks < 0)
			{
				Timer.Start(TimeSpan.FromSeconds(1.0));
			}
			else
			{
				Timer.Start(timeSpan);
			}
		}
	}
}
