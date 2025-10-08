using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.Data;
using GreenT.HornyScapes.Lootboxes;
using GreenT.HornyScapes.MergeField;
using Merge.Meta.RoomObjects;
using StripClub.Model;
using StripClub.NewEvent.Data;
using UniRx;

namespace GreenT.HornyScapes.Events;

[Serializable]
[MementoHolder]
public class Event : IBundleProvider<EventBundleData>, IRewardHolder, ISavableState, IDisposable, IHaveTrackingRewards
{
	[Serializable]
	public class EventMemento : Memento
	{
		public bool IsTutorialEnd;

		public bool IsLaunched;

		public EntityStatus[] RewardStates;

		public bool WasFirstTimeStarted;

		public bool WasFirstTimePushed;

		public EventMemento(Event eventSavable)
			: base(eventSavable)
		{
			IsLaunched = eventSavable.IsLaunched;
			IsTutorialEnd = eventSavable.IsTutorialEnd;
			WasFirstTimeStarted = eventSavable.WasFirstTimeStarted.Value;
			WasFirstTimePushed = eventSavable.WasFirstTimePushed.Value;
			List<EventReward> eventRewards = eventSavable._eventRewards;
			RewardStates = new EntityStatus[eventRewards.Count];
			for (int i = 0; i < eventRewards.Count; i++)
			{
				RewardStates[i] = eventRewards[i].State.Value;
			}
		}
	}

	public readonly int FocusID;

	public readonly bool IsSeparateEnergy;

	public bool IsLaunched;

	public bool IsTutorialEnd;

	private readonly ReactiveProperty<bool> _wasFirstTimeStarted = new ReactiveProperty<bool>();

	private readonly ReactiveProperty<bool> _wasFirstTimePushed = new ReactiveProperty<bool>();

	private readonly MergeFieldMapper _defaultField;

	private readonly List<DropSettings> _previewCards;

	private readonly ReactiveProperty<string> _uniqueKey;

	private readonly EventDataCase _dataCase;

	private readonly IEnumerable<IController> _controllers;

	private List<EventReward> _eventRewards;

	private readonly Subject<NotifyReward> _onRewardUpdate = new Subject<NotifyReward>();

	private IDisposable _anyRewardUpdateStream;

	public readonly int EventId;

	public readonly int CalendarId;

	public readonly int GlobalRatingId;

	public readonly int GroupRatingId;

	public readonly int BattlePassId;

	public EventBundleData Bundle { get; }

	public ViewSettings ViewSettings { get; }

	public MergeFieldMapper DefaultField => _defaultField;

	public IReadOnlyList<DropSettings> PreviewCards => _previewCards;

	public IEnumerable<IController> Controllers => _controllers;

	public bool HasRecipeBook { get; }

	public IReadOnlyReactiveProperty<bool> WasFirstTimeStarted => (IReadOnlyReactiveProperty<bool>)(object)_wasFirstTimeStarted;

	public IReadOnlyReactiveProperty<bool> WasFirstTimePushed => (IReadOnlyReactiveProperty<bool>)(object)_wasFirstTimePushed;

	public EventDataCase Data => _dataCase;

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public Event(int calendarId, int eventId, ViewSettings viewSettings, List<DropSettings> previewCards, List<EventReward> eventReward, MergeFieldMapper defaultField, int focus, bool hasRecipeBook, IBundleData bundleData, IEnumerable<IController> controllers, int globalRatingId, int groupRatingId, bool isSeparateEnergy, int battlePassId)
	{
		EventId = eventId;
		ViewSettings = viewSettings;
		CalendarId = calendarId;
		GlobalRatingId = globalRatingId;
		GroupRatingId = groupRatingId;
		BattlePassId = battlePassId;
		_previewCards = previewCards;
		_eventRewards = eventReward;
		_defaultField = defaultField;
		_controllers = controllers;
		FocusID = focus;
		_dataCase = new EventDataCase();
		Bundle = (EventBundleData)bundleData;
		IsSeparateEnergy = isSeparateEnergy;
		HasRecipeBook = hasRecipeBook;
		_uniqueKey = new ReactiveProperty<string>();
		_uniqueKey.Value = this.EventSaveKey();
		EventSaveMigrationFromEventIDToCalendarId.Instance.AddTomMigration(this);
	}

	public void SetNewSaveKey(string saveKey)
	{
		_uniqueKey.Value = saveKey;
	}

	public void Initialize()
	{
		IObservable<NotifyReward> observable = Observable.Merge<NotifyReward>(_eventRewards.Select((EventReward _reward) => _reward.OnUpdate));
		_anyRewardUpdateStream = ObservableExtensions.Subscribe<NotifyReward>(observable, (Action<NotifyReward>)_onRewardUpdate.OnNext);
	}

	public void InitializeControllers()
	{
		foreach (IController controller in _controllers)
		{
			controller.Initialize();
		}
	}

	public void DisposeControllers()
	{
		foreach (IController controller in _controllers)
		{
			controller.Dispose();
		}
	}

	public string UniqueKey()
	{
		return _uniqueKey.Value;
	}

	public IReadOnlyReactiveProperty<string> GetSaveKey()
	{
		return (IReadOnlyReactiveProperty<string>)(object)_uniqueKey;
	}

	public void SetFirstTimeStarted()
	{
		_wasFirstTimeStarted.Value = true;
	}

	public void SetFirstTimePushed()
	{
		_wasFirstTimePushed.Value = true;
	}

	public IEnumerable<LinkedContent> GetAllRewardsContent()
	{
		return _eventRewards.Select((EventReward reward) => reward.Content);
	}

	public virtual IEnumerable<LinkedContent> GetUncollectedRewardsContent()
	{
		return (from _reward in _eventRewards
			where _reward.State.Value == EntityStatus.Rewarded
			select _reward.Content).ToArray();
	}

	public int GetUncollectedRewardsCount()
	{
		return _eventRewards.Count((EventReward _reward) => _reward.State.Value == EntityStatus.Rewarded);
	}

	public int GetFilteredRewardsCount(IEnumerable<EntityStatus> states)
	{
		return _eventRewards.Count((EventReward _reward) => states.Contains(_reward.State.Value));
	}

	public virtual IEnumerable<BaseReward> GetRewardsToTrack()
	{
		return _eventRewards.ToList();
	}

	public bool HasRewards()
	{
		return _eventRewards.Any((EventReward _rew) => _rew.State.Value == EntityStatus.Rewarded);
	}

	public IObservable<NotifyReward> OnRewardUpdate()
	{
		return Observable.AsObservable<NotifyReward>((IObservable<NotifyReward>)_onRewardUpdate);
	}

	public IReadOnlyList<EventReward> GetEventRewards()
	{
		return _eventRewards;
	}

	public void Dispose()
	{
		_eventRewards.ForEach(delegate(EventReward _entity)
		{
			_entity.Dispose();
		});
		_onRewardUpdate?.OnCompleted();
		_onRewardUpdate?.Dispose();
		_anyRewardUpdateStream?.Dispose();
		_wasFirstTimeStarted?.Dispose();
		_wasFirstTimePushed?.Dispose();
	}

	public Memento SaveState()
	{
		return new EventMemento(this);
	}

	public void LoadState(Memento memento)
	{
		EventMemento eventMemento = (EventMemento)memento;
		IsLaunched = eventMemento.IsLaunched;
		IsTutorialEnd = eventMemento.IsTutorialEnd;
		_wasFirstTimeStarted.Value = eventMemento.WasFirstTimeStarted;
		_wasFirstTimePushed.Value = eventMemento.WasFirstTimePushed;
		if (eventMemento.RewardStates != null && eventMemento.RewardStates.Length == _eventRewards.Count)
		{
			EntityStatus[] rewardStates = eventMemento.RewardStates;
			for (int i = 0; i < _eventRewards.Count; i++)
			{
				_eventRewards[i].State.Value = rewardStates[i];
			}
		}
	}
}
