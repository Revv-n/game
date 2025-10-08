using System;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.Events.Content;
using GreenT.Types;
using StripClub.Model;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes;

public sealed class EventEnergyModeTempService : IInitializable, IDisposable
{
	private ContentSelectorGroup _contentSelectorGroup;

	private CalendarQueue _calendarQueue;

	private EventSettingsProvider _eventSettingsProvider;

	private IDisposable _eventsStateStream;

	public bool IsSeparateEventEnergyMode { get; private set; }

	public EventEnergyModeTempService(ContentSelectorGroup contentSelectorGroup, CalendarQueue calendarQueue, EventSettingsProvider eventSettingsProvider)
	{
		_contentSelectorGroup = contentSelectorGroup;
		_calendarQueue = calendarQueue;
		_eventSettingsProvider = eventSettingsProvider;
	}

	public void Initialize()
	{
		_eventsStateStream = _calendarQueue.OnCalendarActive(EventStructureType.Event).Subscribe(delegate(CalendarModel value)
		{
			TryResetEnergyMode(value.BalanceId);
		});
	}

	public void Dispose()
	{
		_eventsStateStream?.Dispose();
	}

	public CurrencyType TryGetInteractPriceType()
	{
		if (IsSeparateEventEnergyMode)
		{
			if (_contentSelectorGroup.Current != 0 && _contentSelectorGroup.Current != ContentType.BattlePass)
			{
				return CurrencyType.EventEnergy;
			}
			return CurrencyType.Energy;
		}
		return CurrencyType.Energy;
	}

	private void TryResetEnergyMode(int eventId)
	{
		if (_eventSettingsProvider.TryGetEvent(eventId, out var @event))
		{
			IsSeparateEventEnergyMode = @event.IsSeparateEnergy;
		}
	}
}
