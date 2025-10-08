using System;
using GreenT.HornyScapes.BattlePassSpace;
using Merge.Meta.RoomObjects;
using StripClub.Model;
using StripClub.Model.Shop;
using StripClub.NewEvent.Data;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Events;

public class EventRewardTracker : RewardTracker, IInitializable
{
	private IReadOnlyReactiveProperty<int> _eventCurrency;

	private readonly EventProvider _eventProvider;

	private readonly ICurrencyProcessor _currencyProcessor;

	private IDisposable _currencyStream;

	private IDisposable _eventChangeStream;

	public readonly Subject<Unit> Cleared = new Subject<Unit>();

	private EventRewardTracker(ICurrencyProcessor currencyProcessor, EventProvider eventProvider)
	{
		_currencyProcessor = currencyProcessor;
		_eventProvider = eventProvider;
	}

	public void Initialize()
	{
		_eventChangeStream = ObservableExtensions.Subscribe<(CalendarModel, Event)>(Observable.Where<(CalendarModel, Event)>(Observable.Select<(CalendarModel, Event), (CalendarModel, Event)>((IObservable<(CalendarModel, Event)>)_eventProvider.CurrentCalendarProperty, (Func<(CalendarModel, Event), (CalendarModel, Event)>)(((CalendarModel calendar, Event @event) tuple) => tuple)), (Func<(CalendarModel, Event), bool>)(((CalendarModel calendar, Event @event) tuple) => tuple.@event != null && tuple.calendar != null && ((EventMapper)tuple.calendar.EventMapper).bp_id == 0)), (Action<(CalendarModel, Event)>)delegate((CalendarModel calendar, Event @event) tuple)
		{
			OnEventChanged(tuple.@event);
		});
	}

	public void Clear()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (Target != null)
		{
			Target.Value = null;
		}
		Cleared?.OnNext(Unit.Default);
	}

	private void OnEventChanged(Event @event)
	{
		_currencyStream?.Dispose();
		_eventCurrency = _currencyProcessor.GetCountReactiveProperty(CurrencyType.EventXP);
		base.Set(@event);
	}

	protected override void DoOnRewardEmit(BaseReward reward)
	{
		reward.SetRewarded();
		UpdateTargetReward(GetCurrentProgress());
	}

	protected override int GetCurrentProgress()
	{
		return _eventCurrency.Value;
	}

	public override IObservable<BaseReward> OnRewardAchieved(BaseReward reward)
	{
		return Observable.Select<int, BaseReward>(Observable.Where<int>((IObservable<int>)_eventCurrency, (Func<int, bool>)((int _value) => reward != null && reward.State.Value != EntityStatus.Rewarded && reward.Target <= _value)), (Func<int, BaseReward>)((int _) => reward));
	}

	protected override void SetRewardStateOnAchieved(BaseReward reward)
	{
		reward.SetRewarded();
	}

	protected override void SetInProgressStateChooseNextReward(BaseReward reward)
	{
		reward.SetInProgress();
	}

	public override void Dispose()
	{
		base.Dispose();
		_currencyStream?.Dispose();
		_eventChangeStream?.Dispose();
	}
}
