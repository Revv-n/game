using System;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Lootboxes;
using GreenT.Types;
using Merge;
using StripClub;
using StripClub.Model;
using StripClub.NewEvent.Data;
using UnityEngine;

namespace GreenT.HornyScapes.MergeCore;

public class MergePointsIconService
{
	private readonly GameSettings _gameSettings;

	private readonly BattlePassProvider _battlePassProvider;

	private readonly EventProvider _eventProvider;

	private StripClub.GameSettings.CurrencySettingsDictionary Settings => _gameSettings.CurrencySettings;

	public MergePointsIconService(GameSettings gameSettings, EventProvider eventProvider, BattlePassProvider battlePassProvider)
	{
		_gameSettings = gameSettings;
		_eventProvider = eventProvider;
		_battlePassProvider = battlePassProvider;
	}

	public void SetIcon(GameItem item, CurrencySelector dataCurrencySelector)
	{
		Sprite icon = GetIcon(dataCurrencySelector);
		item.MergePointsCase.Activate(icon);
	}

	public Sprite GetIcon(CurrencySelector selector)
	{
		return GetIcon(selector.Currency, selector.Identificator);
	}

	public Sprite GetIcon(CurrencyType currencyType, CompositeIdentificator identificator)
	{
		switch (currencyType)
		{
		case CurrencyType.Soft:
		case CurrencyType.Hard:
		case CurrencyType.Star:
		case CurrencyType.Energy:
		case CurrencyType.XP:
		case CurrencyType.Message:
		case CurrencyType.Real:
		case CurrencyType.MiniEvent:
		case CurrencyType.Jewel:
		case CurrencyType.EventEnergy:
		case CurrencyType.Contracts:
		{
			if (Settings.TryGetValue(currencyType, out var currencySettings, identificator))
			{
				return currencySettings.Sprite;
			}
			return null;
		}
		case CurrencyType.Event:
		case CurrencyType.EventXP:
			return GetEventIcon();
		case CurrencyType.BP:
			return GetBattlePassIcon();
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	private Sprite GetBattlePassIcon()
	{
		return _battlePassProvider.CalendarChangeProperty.Value.Item2?.CurrentViewSettings.Currency;
	}

	private Sprite GetEventIcon()
	{
		return _eventProvider.CurrentCalendarProperty.Value.Item2?.ViewSettings.Currency;
	}
}
