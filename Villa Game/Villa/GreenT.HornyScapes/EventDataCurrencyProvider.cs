using System;
using System.Collections.Generic;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Events;
using StripClub.Model;
using StripClub.NewEvent.Data;
using StripClub.NewEvent.Model;
using UniRx;

namespace GreenT.HornyScapes;

public class EventDataCurrencyProvider : IDisposable
{
	private readonly EventProvider _eventProvider;

	private Dictionary<EventCurrencyType, ReactiveProperty<int>> eventCurrencies;

	private Dictionary<EventCurrencyType, IDisposable> eventIDisposable;

	private Dictionary<EventCurrencyType, EventCurrenciesActionContainer> containers;

	private Event _cachedEvent;

	private IDisposable _eventChangeStream;

	public EventDataCurrencyProvider(EventProvider eventProvider)
	{
		CreateEventDataDictionary();
		_eventProvider = eventProvider;
	}

	public void Initialize()
	{
		_eventChangeStream = _eventProvider.CurrentCalendarProperty.Select(((CalendarModel calendar, Event @event) tuple) => tuple.@event).Subscribe(OnEventDataChanged);
	}

	private void CreateEventDataDictionary()
	{
		eventIDisposable = new Dictionary<EventCurrencyType, IDisposable>();
		eventCurrencies = new Dictionary<EventCurrencyType, ReactiveProperty<int>>
		{
			{
				EventCurrencyType.Core,
				new ReactiveProperty<int>(0)
			},
			{
				EventCurrencyType.XP,
				new ReactiveProperty<int>(0)
			}
		};
		containers = new Dictionary<EventCurrencyType, EventCurrenciesActionContainer>
		{
			{
				EventCurrencyType.Core,
				new EventCurrenciesActionContainer(EventCurrencyType.Core, eventCurrencies[EventCurrencyType.Core], null)
			},
			{
				EventCurrencyType.XP,
				new EventCurrenciesActionContainer(EventCurrencyType.XP, eventCurrencies[EventCurrencyType.XP], null)
			}
		};
	}

	public bool TryGetContainer(CurrencyType type, out ICurrenciesActionContainer container)
	{
		container = null;
		if (!type.TryToEvent(out var eventCurrencyType))
		{
			return false;
		}
		container = containers[eventCurrencyType];
		return container != null;
	}

	private void OnEventDataChanged(Event newEvent)
	{
		CleanEventData(_cachedEvent);
		SubscribeNewEventData(newEvent);
	}

	private void SubscribeNewEventData(Event @event)
	{
		if (@event?.Data?.Wallet != null)
		{
			SubscribeEventCurrency(@event.Data, EventCurrencyType.XP);
			SubscribeEventCurrency(@event.Data, EventCurrencyType.Core);
			containers[EventCurrencyType.XP].UpdateWallet(@event.Data.Wallet);
			containers[EventCurrencyType.Core].UpdateWallet(@event.Data.Wallet);
		}
		else
		{
			foreach (ReactiveProperty<int> value in eventCurrencies.Values)
			{
				value.Value = 0;
			}
		}
		_cachedEvent = @event;
	}

	private void CleanEventData(Event @event)
	{
		if (@event?.Data?.Wallet == null)
		{
			return;
		}
		foreach (IDisposable value in eventIDisposable.Values)
		{
			value.Dispose();
		}
		containers[EventCurrencyType.Core].UpdateWallet(null);
		containers[EventCurrencyType.XP].UpdateWallet(null);
		eventIDisposable.Clear();
	}

	private void SubscribeEventCurrency(EventDataCase @case, EventCurrencyType type)
	{
		IDisposable value = @case.Wallet.Get(type).Subscribe(delegate(int x)
		{
			OnChangeValue(type, x);
		});
		eventIDisposable.Add(type, value);
	}

	private void OnChangeValue(EventCurrencyType type, int i)
	{
		eventCurrencies[type].Value = i;
	}

	public void Dispose()
	{
		_eventChangeStream?.Dispose();
	}
}
