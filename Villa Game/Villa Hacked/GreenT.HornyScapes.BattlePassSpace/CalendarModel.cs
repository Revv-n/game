using System;
using GreenT.Data;
using GreenT.HornyScapes._HornyScapes._Scripts.Events.Calendar;
using GreenT.HornyScapes.Events;
using Merge.Meta.RoomObjects;
using StripClub.Extensions;
using StripClub.Model;
using StripClub.UI;
using UniRx;
using UniRx.Operators;

namespace GreenT.HornyScapes.BattlePassSpace;

[MementoHolder]
public abstract class CalendarModel : IDisposable, ISavableState
{
	[Serializable]
	public abstract class CalendarMemento : Memento
	{
		public bool WasStarted;

		public bool WasEnded;

		public bool EventEnergyResetToStart;

		public EntityStatus EntityStatus;

		public CalendarMemento(CalendarModel savableState)
			: base(savableState)
		{
			Save(savableState);
		}

		public CalendarMemento Save(CalendarModel savableState)
		{
			WasStarted = savableState.WasStarted;
			WasEnded = savableState.WasEnded;
			EventEnergyResetToStart = savableState.EventEnergyResetToStart;
			EntityStatus = savableState.CalendarState.Value;
			return this;
		}
	}

	private ICalendarStateStrategy _calendarStateStrategy;

	private readonly Subject<CalendarModel> _onGotReward = new Subject<CalendarModel>();

	protected readonly IClock clock;

	protected readonly int duration;

	protected readonly CompositeDisposable _timerStream = new CompositeDisposable();

	public bool IsLoading;

	public bool WasStarted;

	public bool WasEnded;

	public bool EventEnergyResetToStart;

	public readonly int UniqID;

	public readonly IEventMapper EventMapper;

	public readonly GenericTimer ComingSoonTimer;

	public readonly GenericTimer LastChanceTimer;

	public readonly ReactiveProperty<EntityStatus> CalendarState;

	public readonly ReactiveProperty<CalendarModelLoadingState> CalendarLoadingState;

	public readonly ReactiveProperty<CalendarLoadingStatus> LoadingStatus;

	public readonly ILocker Locker;

	public readonly long LastChanceDuration;

	public GenericTimer Duration;

	public IRewardHolder RewardHolder;

	public int BalanceId { get; private set; }

	public EventStructureType EventType { get; }

	public long StartedTimeStamp { get; protected set; }

	public long ComingSoonTimeStamp { get; protected set; }

	public long LastChanceTimeStamp { get; protected set; }

	public ICalendarStrategy CalendarStrategy { get; private set; }

	public ReadOnlyReactiveProperty<bool> IsUnlockedAndNotPassed { get; protected set; }

	public int DurationTime => duration;

	public bool IsActiveTime
	{
		get
		{
			if (StartedTimeStamp != 0L && RemainingTime <= 0)
			{
				return LastChanceTimeStamp >= clock.GetTime().ConvertToUnixTimestamp();
			}
			return true;
		}
	}

	public IObservable<CalendarModel> OnGotReward => Observable.AsObservable<CalendarModel>((IObservable<CalendarModel>)_onGotReward);

	public virtual bool IsNotPassedCalendar => CalendarState.Value != EntityStatus.Rewarded;

	public long RemainingTime => StartedTimeStamp - clock.GetTime().ConvertToUnixTimestamp() + duration;

	public long EndTime => StartedTimeStamp + duration;

	public SavableStatePriority Priority => SavableStatePriority.Base;

	protected CalendarModel(int uniqId, EventStructureType structureType, IEventMapper eventMapper, int duration, ILocker[] locker, IClock clock, ICalendarStrategy calendarStrategy, long lastChanceDuration)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		EventType = structureType;
		ComingSoonTimer = new GenericTimer(TimeSpan.Zero);
		Duration = new GenericTimer(TimeSpan.Zero);
		LastChanceTimer = new GenericTimer(TimeSpan.Zero);
		CalendarState = new ReactiveProperty<EntityStatus>(EntityStatus.Blocked);
		CalendarLoadingState = new ReactiveProperty<CalendarModelLoadingState>(CalendarModelLoadingState.None);
		LoadingStatus = new ReactiveProperty<CalendarLoadingStatus>(CalendarLoadingStatus.None);
		Locker = new CompositeLocker(locker);
		UniqID = uniqId;
		LastChanceDuration = lastChanceDuration;
		EventMapper = eventMapper;
		CalendarStrategy = calendarStrategy;
		this.duration = duration;
		this.clock = clock;
	}

	public virtual void Initialize()
	{
		IsUnlockedAndNotPassed = ReactivePropertyExtensions.ToReadOnlyReactiveProperty<bool>(Observable.CombineLatest<bool, EntityStatus, CalendarModelLoadingState, bool>((IObservable<bool>)Locker.IsOpen, (IObservable<EntityStatus>)CalendarState, (IObservable<CalendarModelLoadingState>)CalendarLoadingState, (CombineLatestFunc<bool, EntityStatus, CalendarModelLoadingState, bool>)((bool _isOpen, EntityStatus _calendarState, CalendarModelLoadingState _calendarLoadingState) => IsNotPassedCalendar && _isOpen && _calendarLoadingState != CalendarModelLoadingState.Failed)), false);
	}

	public void Set(IRewardHolder rewardHolder, ICalendarStateStrategy calendarStateStrategy, int balanceId)
	{
		RewardHolder = rewardHolder;
		BalanceId = balanceId;
		_calendarStateStrategy = calendarStateStrategy;
	}

	public virtual void Launch()
	{
		WasStarted = true;
	}

	protected void SetInProgress()
	{
		if (RemainingTime > 0)
		{
			_calendarStateStrategy.OnInProgress();
			Duration.Start(TimeSpan.FromSeconds(RemainingTime));
			ObservableExtensions.Subscribe<GenericTimer>(Observable.Take<GenericTimer>(Duration.OnTimeIsUp, 1), (Action<GenericTimer>)delegate
			{
				SetComplete();
			});
			CalendarState.Value = EntityStatus.InProgress;
		}
		else
		{
			SetComplete();
		}
	}

	protected void SetComplete()
	{
		if (_calendarStateStrategy.CheckIfRewarded())
		{
			WasStarted = false;
			CalendarState.Value = EntityStatus.Rewarded;
			_calendarStateStrategy.OnRewarded();
		}
		else
		{
			CalendarState.Value = EntityStatus.Complete;
			_calendarStateStrategy.OnComplete();
		}
	}

	public void SetLoadingStatus(CalendarModelLoadingState newLoadingState)
	{
		if (newLoadingState != CalendarLoadingState.Value)
		{
			if (newLoadingState == CalendarModelLoadingState.Failed || newLoadingState == CalendarModelLoadingState.Success)
			{
				IsLoading = false;
			}
			CalendarLoadingState.Value = newLoadingState;
		}
	}

	public void SetLoadingLifeStatus(CalendarLoadingStatus status)
	{
		LoadingStatus.Value = status;
	}

	public void SetRewarded()
	{
		_calendarStateStrategy.OnRewarded();
		_onGotReward.OnNext(this);
		CalendarState.Value = EntityStatus.Rewarded;
	}

	public override string ToString()
	{
		return GetType().Name + " ID: " + UniqID;
	}

	public virtual void Dispose()
	{
		_onGotReward.OnCompleted();
		_onGotReward.Dispose();
		Duration?.Dispose();
		CompositeDisposable timerStream = _timerStream;
		if (timerStream != null)
		{
			timerStream.Dispose();
		}
		ComingSoonTimer?.Dispose();
		LastChanceTimer?.Dispose();
		CalendarState?.Dispose();
		IsUnlockedAndNotPassed.Dispose();
	}

	public abstract string UniqueKey();

	public abstract Memento SaveState();

	public abstract void LoadState(Memento memento);
}
