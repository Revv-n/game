using GreenT.HornyScapes;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.Events.Content;
using GreenT.Types;
using StripClub.Model;
using UnityEngine;
using Zenject;

namespace StripClub.UI;

public class EventCurrencySpriteAttacher : CurrencySpriteAttacher
{
	[SerializeField]
	private CurrencyType alternativeCurrencyType;

	[Inject]
	private ContentSelectorGroup selectorGroup;

	[Inject]
	private CalendarQueue calendarQueue;

	protected virtual void OnEnable()
	{
		SetView();
	}

	public override void SetView()
	{
		CurrencyType currencyType = ((selectorGroup.Current == ContentType.Main) ? currency : alternativeCurrencyType);
		if (TryGetEventComposite(out var compositeIdentificator))
		{
			SetView(compositeIdentificator);
		}
		else if (gameSettings.CurrencySettings.Contains(currencyType, default(CompositeIdentificator)))
		{
			target.sprite = gameSettings.CurrencySettings[currencyType, default(CompositeIdentificator)].Sprite;
		}
		else
		{
			target.sprite = gameSettings.CurrencySettings[currencyType, default(CompositeIdentificator)].Sprite;
		}
	}

	public override void SetView(CompositeIdentificator compositeIdentificator = default(CompositeIdentificator))
	{
		CurrencyType currencyType = ((selectorGroup.Current == ContentType.Main) ? currency : alternativeCurrencyType);
		if (gameSettings.CurrencySettings.Contains(currencyType, compositeIdentificator))
		{
			target.sprite = gameSettings.CurrencySettings[currencyType, compositeIdentificator].Sprite;
		}
		else if (gameSettings.CurrencySettings.Contains(currencyType, default(CompositeIdentificator)))
		{
			target.sprite = gameSettings.CurrencySettings[currencyType, default(CompositeIdentificator)].Sprite;
		}
		else
		{
			target.sprite = gameSettings.CurrencySettings[currencyType, default(CompositeIdentificator)].Sprite;
		}
	}

	private bool TryGetEventComposite(out CompositeIdentificator compositeIdentificator)
	{
		CalendarModel activeCalendar = calendarQueue.GetActiveCalendar(EventStructureType.Event);
		if (activeCalendar == null)
		{
			compositeIdentificator = default(CompositeIdentificator);
			return false;
		}
		if (activeCalendar.EventMapper is EventMapper { group_rating_id: not 0 })
		{
			compositeIdentificator = new CompositeIdentificator(activeCalendar.BalanceId);
			return true;
		}
		compositeIdentificator = default(CompositeIdentificator);
		return false;
	}
}
