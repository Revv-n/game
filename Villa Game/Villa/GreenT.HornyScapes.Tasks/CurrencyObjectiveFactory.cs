using System;
using System.Linq;
using System.Text.RegularExpressions;
using GreenT.Data;
using GreenT.HornyScapes.MergeCore;
using GreenT.HornyScapes.MergeStore;
using GreenT.HornyScapes.Tasks.Data;
using GreenT.Types;
using StripClub.Model;
using StripClub.Model.Shop;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Tasks;

public class CurrencyObjectiveFactory : ConcreteObjectiveBaseFactory
{
	private const string SPEND_SOFT_RULE = "spendSoft";

	private const string SPEND_HARD_RULE = "spendHard";

	private const string SPEND_EVENT_RULE = "spendEvent";

	private const string SPEND_ENERGY_RULE = "spendEnergy";

	private const string SPEND_HARD_SHOP_RULE = "spendHardShop";

	private const string SPEND_HARD_RECHARGE_RULE = "spendHardRecharge";

	private const string SPEND_HARD_BUBBLR_RULE = "spendHardBubble";

	private const string GIVE_LOVE_POINTS_RULE = "giveLovePoints";

	private const string BUY_SHOP_RULE = "buyShop";

	private const string SPEND_CONCRETE_MINIEVENT_RULE = "spend_conc_minievent:(\\d+)";

	private const string SPEND_MINIEVENT_RULE = "spend_any_minievent";

	private const string GET_SOFT_RULE = "getSoft";

	private const string GET_HARD_RULE = "getHard";

	private const string GET_CONCRETE_MINIEVENT_RULE = "get_conc_minievent:(\\d+)";

	private const string GET_MINIEVENT_RULE = "get_any_minievent";

	private const string GET_CONCRETE_EVENTXP_RULE = "get_conc_eventXP:(\\d+)";

	private const string GET_CONCRETE_EVENT_RULE = "get_conc_eventCurr:(\\d+)";

	private const string GET_CONCRETE_EVENTXP_COMPARER = "get_conc_eventXP:";

	private const string GET_CONCRETE_EVENT_COMPARER = "get_conc_eventCurr:";

	private const string SPEND_CONCRETE_MINIEVENT_COMPARER = "spend_conc_minievent:";

	private const string GET_CONCRETE_MINIEVENT_COMPARER = "get_conc_minievent:";

	private readonly TrackableCurrencyActionContainerTracker _trackableCurrencyActionContainerTracker;

	private readonly SignalBus _signalBus;

	private readonly CalendarQueue _calendarQueue;

	public CurrencyObjectiveFactory(CalendarQueue calendarQueue, TrackableCurrencyActionContainerTracker trackableCurrencyActionContainerTracker, TaskObjectiveIcons objectiveIcons, GameSettings gameSettings, ICurrencyProcessor currencyProcessor, ISaver saver, SignalBus signalBus)
		: base(objectiveIcons, gameSettings, currencyProcessor, saver, "spendSoft|spendEvent|spendEnergy|getSoft|getHard|spend_conc_minievent:(\\d+)|\r\nspend_any_minievent|get_conc_minievent:(\\d+)|get_any_minievent|get_conc_eventXP:(\\d+)|get_conc_eventCurr:(\\d+)|spendHard|giveLovePoints")
	{
		_calendarQueue = calendarQueue;
		_trackableCurrencyActionContainerTracker = trackableCurrencyActionContainerTracker;
		_signalBus = signalBus;
	}

	public override IObjective Create(TaskMapper mapper, int index, ContentType contentType)
	{
		Match match = _objectiveRegex.Match(mapper.req_items[index]);
		if (TryCreateMergeStoreObjective(mapper, index, out var objective))
		{
			return objective;
		}
		try
		{
			if (match.Success)
			{
				switch (match.Value)
				{
				case "spendSoft":
					return CreateSpendCurrencyObjective(mapper, mapper.req_value[index], GetCurrencyIcon(CurrencyType.Soft), CurrencyType.Soft);
				case "spendHard":
					return CreateSpendCurrencyObjective(mapper, mapper.req_value[index], GetCurrencyIcon(CurrencyType.Hard), CurrencyType.Hard);
				case "spendEvent":
					return CreateSpendCurrencyObjective(mapper, mapper.req_value[index], GetCurrencyIcon(CurrencyType.Event), CurrencyType.Event);
				case "spendEnergy":
					return CreateSpendCurrencyObjective(mapper, mapper.req_value[index], GetCurrencyIcon(CurrencyType.Energy), CurrencyType.Energy);
				case "spend_any_minievent":
					return CreateSpendCurrencyObjective(mapper, mapper.req_value[index], GetCurrencyIcon(CurrencyType.MiniEvent), CurrencyType.MiniEvent);
				case "getSoft":
					return CreateAddCurrencyObjective(mapper, mapper.req_value[index], GetCurrencyIcon(CurrencyType.Soft), CurrencyType.Soft);
				case "getHard":
					return CreateAddCurrencyObjective(mapper, mapper.req_value[index], GetCurrencyIcon(CurrencyType.Hard), CurrencyType.Hard);
				case "get_any_minievent":
					return CreateAddCurrencyObjective(mapper, mapper.req_value[index], GetCurrencyIcon(CurrencyType.MiniEvent), CurrencyType.MiniEvent);
				case "giveLovePoints":
					return CreateAddCurrencyObjective(mapper, mapper.req_value[index], GetCurrencyIcon(CurrencyType.LovePoints), CurrencyType.LovePoints);
				}
			}
			if (match.Value.Contains("get_conc_minievent:"))
			{
				return CreateAddCurrencyObjective(mapper, mapper.req_value[index], GetCurrencyIcon(CurrencyType.MiniEvent), CurrencyType.MiniEvent, isConcrete: true);
			}
			if (match.Value.Contains("spend_conc_minievent:"))
			{
				return CreateSpendCurrencyObjective(mapper, mapper.req_value[index], GetCurrencyIcon(CurrencyType.MiniEvent), CurrencyType.MiniEvent, isConcrete: true);
			}
			if (match.Value.Contains("get_conc_eventXP:"))
			{
				return CreateAddCurrencyObjective(mapper, mapper.req_value[index], GetCurrencyIcon(CurrencyType.Event), CurrencyType.EventXP, isConcrete: true);
			}
			if (match.Value.Contains("get_conc_eventCurr:"))
			{
				return CreateAddCurrencyObjective(mapper, mapper.req_value[index], GetCurrencyIcon(CurrencyType.Event), CurrencyType.Event, isConcrete: true);
			}
			return null;
		}
		catch (Exception innerException)
		{
			throw innerException.SendException($"{GetType().Name}: Can't create Objective {mapper.task_id}" + " from " + mapper.req_items[index]);
		}
	}

	private bool TryCreateMergeStoreObjective(TaskMapper mapper, int index, out IObjective objective)
	{
		objective = null;
		string text = mapper.req_items[index];
		if (string.IsNullOrEmpty(text))
		{
			return false;
		}
		if (index < 0 || index >= mapper.req_value.Length)
		{
			return false;
		}
		switch (text)
		{
		case "spendHardShop":
			objective = CreateMergeStoreObjective<SpendHardInMergeStoreObjective, SpendHardMergeStoreSignal>(mapper, mapper.req_value[index], GetCurrencyIcon(CurrencyType.Hard));
			return true;
		case "spendHardRecharge":
			objective = CreateMergeStoreObjective<SpendHardInForRechargeObjective, SpendHardForRechargeSignal>(mapper, mapper.req_value[index], GetCurrencyIcon(CurrencyType.Hard));
			return true;
		case "spendHardBubble":
			objective = CreateMergeStoreObjective<SpendHardForOpenBubbleObjective, SpendHardBubbleSignal>(mapper, mapper.req_value[index], GetCurrencyIcon(CurrencyType.Hard));
			return true;
		case "buyShop":
			objective = CreateMergeStoreObjective<BuyInMergeStoreObjective, BuyItemInMergeStoreSignal>(mapper, mapper.req_value[index], GetCurrencyIcon(CurrencyType.Hard));
			return true;
		default:
			return false;
		}
	}

	private T CreateMergeStoreObjective<T, TW>(TaskMapper mapper, int reqValue, Func<Sprite> icon) where T : MergeStoreObjective<TW> where TW : IValuableSignal
	{
		SavableObjectiveData savableObjectiveData = CreateGainData(mapper.task_id, reqValue);
		T result = (T)Activator.CreateInstance(typeof(T), icon, savableObjectiveData, _signalBus);
		_saver.Add(savableObjectiveData);
		return result;
	}

	private GainObjective CreateSpendCurrencyObjective(TaskMapper mapper, int reqValue, Func<Sprite> icon, CurrencyType currencyType, bool isConcrete = false)
	{
		SavableObjectiveData savableObjectiveData = CreateGainData(mapper.task_id, reqValue);
		int[] array = new int[1];
		GainObjective result;
		if (currencyType == CurrencyType.MiniEvent && !isConcrete)
		{
			result = new AnyCurrencySpendObjective(_trackableCurrencyActionContainerTracker, icon, currencyType, savableObjectiveData, _currencyProcessor);
		}
		else
		{
			if (currencyType == CurrencyType.MiniEvent)
			{
				Match match = Regex.Match(mapper.req_items.First(), "spend_conc_minievent:(\\d+)");
				if (match.Success)
				{
					array[0] = int.Parse(match.Groups[1].Value);
				}
			}
			result = new ConcreteCurrencySpendObjective(icon, currencyType, savableObjectiveData, _currencyProcessor, array);
		}
		_saver.Add(savableObjectiveData);
		return result;
	}

	private GainObjective CreateAddCurrencyObjective(TaskMapper mapper, int reqValue, Func<Sprite> icon, CurrencyType currencyType, bool isConcrete = false)
	{
		SavableObjectiveData savableObjectiveData = CreateGainData(mapper.task_id, reqValue);
		int[] array = new int[1];
		GainObjective result;
		if ((currencyType == CurrencyType.MiniEvent || currencyType == CurrencyType.LovePoints) && !isConcrete)
		{
			result = new AnyCurrencyAddObjective(_trackableCurrencyActionContainerTracker, icon, currencyType, savableObjectiveData, _currencyProcessor);
		}
		else
		{
			if (currencyType == CurrencyType.MiniEvent)
			{
				Match match = Regex.Match(mapper.req_items.First(), "get_conc_minievent:(\\d+)");
				if (match.Success)
				{
					array[0] = int.Parse(match.Groups[1].Value);
				}
			}
			int target_event_id = 0;
			if (currencyType == CurrencyType.EventXP || currencyType == CurrencyType.Event)
			{
				target_event_id = TryParseTargetEventId(mapper.req_items.First(), (currencyType == CurrencyType.EventXP) ? "get_conc_eventXP:(\\d+)" : "get_conc_eventCurr:(\\d+)");
			}
			result = currencyType switch
			{
				CurrencyType.EventXP => new EventXPAddCurrencyObjective(_calendarQueue, target_event_id, icon, currencyType, savableObjectiveData, _currencyProcessor, array), 
				CurrencyType.Event => new EventAddCurrencyObjective(_calendarQueue, target_event_id, icon, currencyType, savableObjectiveData, _currencyProcessor, array), 
				_ => new ConcreteCurrencyAddObjective(icon, currencyType, savableObjectiveData, _currencyProcessor, array), 
			};
		}
		_saver.Add(savableObjectiveData);
		return result;
	}

	private int TryParseTargetEventId(string value, string rule)
	{
		int result = 0;
		Match match = Regex.Match(value, rule);
		if (match.Success)
		{
			result = int.Parse(match.Groups[1].Value);
		}
		return result;
	}

	private Func<Sprite> GetCurrencyIcon(CurrencyType currencyType)
	{
		return currencyType switch
		{
			CurrencyType.Soft => () => _objectiveIcons.SpendSoftObjective, 
			CurrencyType.Hard => () => _objectiveIcons.SpendHardObjective, 
			CurrencyType.Event => () => _gameSettings.CurrencySettings[CurrencyType.Event, default(CompositeIdentificator)].Sprite, 
			CurrencyType.Energy => () => _objectiveIcons.SpendSoftObjective, 
			CurrencyType.MiniEvent => () => _objectiveIcons.SpendSoftObjective, 
			CurrencyType.LovePoints => () => _objectiveIcons.GiveLovePoints, 
			_ => null, 
		};
	}
}
