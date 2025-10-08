using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.Data;
using GreenT.HornyScapes.MiniEvents;
using GreenT.Types;
using JetBrains.Annotations;
using Merge.Meta.RoomObjects;
using StripClub.Model;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Events;

[MementoHolder]
public sealed class MiniEvent : IRewardHolder, IIdentifiable, IDisposable, ISavableState
{
	[Serializable]
	private class MiniEventMemento : Memento
	{
		public bool StartWindowShown;

		public bool WasFirstTimeSeen;

		public MiniEventMemento(MiniEvent savableMinievent)
			: base(savableMinievent)
		{
			StartWindowShown = savableMinievent.StartWindowShown;
			WasFirstTimeSeen = savableMinievent.WasFirstTimeSeen.Value;
		}
	}

	public struct PromoSettings
	{
		public string PromoView;

		public List<LinkedContent> PromoContent;
	}

	public struct ViewSettings
	{
		public Sprite Tab;

		public Sprite Background;

		[CanBeNull]
		public Sprite Girl;
	}

	private const string _dataSaveKey = "mini_event_data_";

	public bool IsLaunched;

	public readonly int CalendarId;

	public readonly int BalanceId;

	public readonly int EventId;

	public readonly bool IsMultiTabbed;

	public readonly PromoSettings Promo;

	public readonly ViewSettings? ViewSetting;

	private readonly ReactiveProperty<string> _uniqueKey;

	private readonly IEnumerable<IController> _controllers;

	private readonly List<EventReward> allRewards;

	private readonly Subject<NotifyReward> _onRewardUpdate = new Subject<NotifyReward>();

	private IDisposable _anyRewardUpdateStream;

	public bool StartWindowShown { get; set; }

	public ReactiveProperty<bool> IsSpawned { get; set; }

	public ReactiveProperty<bool> WasFirstTimeSeen { get; set; }

	public ReactiveProperty<bool> IsAnyContentAvailable { get; set; }

	public int PriorityId { get; set; }

	public GreenT.HornyScapes.MiniEvents.ConfigType ConfigType { get; private set; }

	public CompositeIdentificator Identificator { get; private set; }

	public CompositeIdentificator CurrencyIdentificator { get; private set; }

	public IEnumerable<IController> Controllers => _controllers;

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public MiniEvent(int calendarId, int position, int balanceId, int eventId, bool isMultiTabbed, IEnumerable<IController> controllers, CompositeIdentificator currencyIdentificator, ViewSettings? viewSettings, PromoSettings promoSettings, GreenT.HornyScapes.MiniEvents.ConfigType configType)
	{
		PriorityId = position;
		Identificator = new CompositeIdentificator(eventId, balanceId);
		CurrencyIdentificator = currencyIdentificator;
		BalanceId = balanceId;
		EventId = eventId;
		IsMultiTabbed = isMultiTabbed;
		Promo = promoSettings;
		ViewSetting = viewSettings;
		CalendarId = calendarId;
		ConfigType = configType;
		_controllers = controllers;
		WasFirstTimeSeen = new ReactiveProperty<bool>();
		IsSpawned = new ReactiveProperty<bool>();
		IsAnyContentAvailable = new ReactiveProperty<bool>();
		_uniqueKey = new ReactiveProperty<string>();
		_uniqueKey.Value = GenerateSaveKey(BalanceId, EventId);
	}

	public void Initialize()
	{
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
		IsAnyContentAvailable.Value = false;
	}

	public void RefreshSaveState()
	{
		WasFirstTimeSeen.Value = false;
		StartWindowShown = false;
	}

	public bool HasRewards()
	{
		return false;
	}

	public IEnumerable<LinkedContent> GetAllRewardsContent()
	{
		return allRewards.Select((EventReward reward) => reward.Content);
	}

	public IEnumerable<LinkedContent> GetUncollectedRewardsContent()
	{
		throw new NotImplementedException();
	}

	public int GetFilteredRewardsCount(IEnumerable<EntityStatus> states)
	{
		throw new NotImplementedException();
	}

	public void Dispose()
	{
		allRewards.ForEach(delegate(EventReward _entity)
		{
			_entity.Dispose();
		});
		_onRewardUpdate?.OnCompleted();
		_onRewardUpdate?.Dispose();
		_anyRewardUpdateStream?.Dispose();
		foreach (IController controller in _controllers)
		{
			controller.Dispose();
		}
	}

	public string UniqueKey()
	{
		return _uniqueKey.Value;
	}

	public Memento SaveState()
	{
		return new MiniEventMemento(this);
	}

	private string GenerateSaveKey(int balanceId, int eventId)
	{
		return string.Format("{0}{1}{2}", "mini_event_data_", balanceId, eventId);
	}

	public void LoadState(Memento memento)
	{
		MiniEventMemento miniEventMemento = (MiniEventMemento)memento;
		StartWindowShown = miniEventMemento.StartWindowShown;
		WasFirstTimeSeen.Value = miniEventMemento.WasFirstTimeSeen;
	}
}
