using System;
using GreenT;
using StripClub.Model;

namespace StripClub.NewEvent.Model;

public static class EventCurrencyTypeExtension
{
	public static bool TryToCore(this EventCurrencyType eventCurrencyType, out CurrencyType currencyType)
	{
		currencyType = CurrencyType.None;
		switch (eventCurrencyType)
		{
		case EventCurrencyType.XP:
			currencyType = CurrencyType.EventXP;
			return true;
		case EventCurrencyType.Core:
			currencyType = CurrencyType.Event;
			return true;
		default:
			new ArgumentException($"Error converting EventCurrencyType {eventCurrencyType} to CurrencyType").LogException();
			return false;
		}
	}

	public static bool TryToEvent(this CurrencyType currencyType, out EventCurrencyType eventCurrencyType)
	{
		eventCurrencyType = EventCurrencyType.None;
		switch (currencyType)
		{
		case CurrencyType.Event:
			eventCurrencyType = EventCurrencyType.Core;
			return true;
		case CurrencyType.EventXP:
			eventCurrencyType = EventCurrencyType.XP;
			return true;
		default:
			new ArgumentException($"Error converting CurrencyType {currencyType} to EventCurrencyType").LogException();
			return false;
		}
	}
}
