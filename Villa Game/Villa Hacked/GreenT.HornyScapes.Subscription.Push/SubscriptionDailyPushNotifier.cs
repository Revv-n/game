using System;
using System.Collections.Generic;
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
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		Model = model;
		Timer.TimeLeft = TimeSpan.Zero;
		Lot = lotManager.Collection.First((Lot lot) => lot is SubscriptionLot && lot.ID == Model.BaseID) as SubscriptionLot;
		_clock = clock;
	}

	public void Set()
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Unit>((IObservable<Unit>)_onClaimSubject, (Action<Unit>)delegate
		{
			StartTimer(RechargeTime);
		}), (ICollection<IDisposable>)_compositeDisposable);
	}

	private void StartTimer(TimeSpan pushTime)
	{
		Timer.Start(pushTime);
	}

	public IObservable<Unit> OnPushRequest(bool force = false)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if (force && Timer.TimeLeft == TimeSpan.Zero)
		{
			return Observable.Return(default(Unit));
		}
		return Observable.AsUnitObservable<GenericTimer>(Timer.OnTimeIsUp);
	}

	public void SetClaimed()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		_onClaimSubject.OnNext(default(Unit));
	}

	public DateTime GetTime()
	{
		return _clock.GetTime();
	}

	public void Dispose()
	{
		CompositeDisposable compositeDisposable = _compositeDisposable;
		if (compositeDisposable != null)
		{
			compositeDisposable.Dispose();
		}
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
