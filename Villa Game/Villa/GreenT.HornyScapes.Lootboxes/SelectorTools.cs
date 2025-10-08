using System;
using System.Text.RegularExpressions;
using GreenT.Types;
using StripClub.Extensions;
using StripClub.Model;
using StripClub.Model.Cards;

namespace GreenT.HornyScapes.Lootboxes;

public static class SelectorTools
{
	public static Selector CreateSelector(string selectorString)
	{
		if (int.TryParse(selectorString, out var result))
		{
			return new SelectorByID(result);
		}
		string[] array = selectorString.Split(':');
		CompositeIdentificator identificator;
		if (array.Length == 2)
		{
			int[] identificators = Array.ConvertAll(array[1].Split(','), int.Parse);
			identificator = new CompositeIdentificator(identificators);
		}
		else
		{
			identificator = new CompositeIdentificator(default(int));
		}
		Match match = new Regex("soft_money|hard_money|event_money|event_energy|energy|stars|event_xp|bp_points|minievent_money|jewels|contracts|present_1|present_2|present_3|present_4").Match(array[0]);
		if (match.Success)
		{
			GetResourceEnumValueByConfigKey(match.Value, out var currency);
			return new CurrencySelector(currency, identificator);
		}
		match = new Regex("min_lvl|max_lvl|enable_from|disable_from|girl_not_in_collection|girl_in_collection|lot_bought|lot_not_bought|level|task_complete|task_rewarded|step_complete|character_min_promote|character_max_promote|dialogue_complete|room_object|tutor_id_complete|story_complete|resource_qty_max_star|step_not_rewarded|task_not_rewarded|first_purchase|min_payment_count|max_payment_count|payment_count|min_purchases_in_section|max_purchases_in_section|purchases_in_section|dialogue_not_completed|event_start|event_in_progress|event_rewarded|event_target|enable_from_event|disable_from_event|bp_level|bp_level_range|bp_in_progress|any_lot_bought|event_target_range|unreachable|mini_event_in_progress|sum_price_average|roulette_roll_greater|roulette_roll_less|subscription_active|subscription_not_active|roulette_bank_roll_greater|roulette_bank_roll_less|merge_item|summon_use|spend_event_energy|relationship_rewarded").Match(array[0]);
		if (match.Success)
		{
			return new UnlockTypeSelector(ParseUnlockType(match.Value), identificator);
		}
		Match match2 = new Regex("battle_pass").Match(selectorString);
		if (match2.Success)
		{
			if (match2.Value == "battle_pass")
			{
				LevelType level = LevelType.BattlePass;
				return new LevelSelector(level);
			}
			throw new ArgumentOutOfRangeException("How did you get here? Probably you have problems with you hardware. MatchValue: " + match2.Value);
		}
		string[] array2 = selectorString.Split('&');
		if (array2.Length == 2)
		{
			string text = array2[0].Split(':')[1].FirstCharToUpper();
			string obj = array2[1].Split(':')[1];
			if (!Enum.TryParse<Rarity>(text, out var result2))
			{
				throw new ArgumentException().SendException("Can't parse rarity from string: \"" + text + "\"");
			}
			CardSelector.TargetPool pool = (obj.Contains("not") ? CardSelector.TargetPool.Out : CardSelector.TargetPool.In);
			return new CardSelector(result2, pool);
		}
		throw new ArgumentException("Couldn't parse selector string:\"" + selectorString + "\"");
	}

	public static void GetResourceEnumValueByConfigKey(string key, out CurrencyType currency)
	{
		switch (key)
		{
		case "soft_money":
			currency = CurrencyType.Soft;
			break;
		case "hard_money":
			currency = CurrencyType.Hard;
			break;
		case "event_money":
			currency = CurrencyType.Event;
			break;
		case "energy":
			currency = CurrencyType.Energy;
			break;
		case "event_energy":
			currency = CurrencyType.EventEnergy;
			break;
		case "stars":
			currency = CurrencyType.Star;
			break;
		case "event_xp":
			currency = CurrencyType.EventXP;
			break;
		case "bp_points":
			currency = CurrencyType.BP;
			break;
		case "minievent_money":
			currency = CurrencyType.MiniEvent;
			break;
		case "jewels":
			currency = CurrencyType.Jewel;
			break;
		case "contracts":
			currency = CurrencyType.Contracts;
			break;
		case "present_1":
			currency = CurrencyType.Present1;
			break;
		case "present_2":
			currency = CurrencyType.Present2;
			break;
		case "present_3":
			currency = CurrencyType.Present3;
			break;
		case "present_4":
			currency = CurrencyType.Present4;
			break;
		default:
			throw new ArgumentOutOfRangeException("Wrong currency name: " + key);
		}
	}

	public static string GetResourceNameValueByType(CurrencyType currencyType)
	{
		return currencyType switch
		{
			CurrencyType.Soft => "soft_money", 
			CurrencyType.Hard => "hard_money", 
			CurrencyType.Event => "event_money", 
			CurrencyType.Energy => "energy", 
			CurrencyType.EventEnergy => "event_energy", 
			CurrencyType.Star => "stars", 
			CurrencyType.EventXP => "event_xp", 
			CurrencyType.BP => "bp_points", 
			CurrencyType.MiniEvent => "minievent_money", 
			CurrencyType.Jewel => "jewels", 
			CurrencyType.Contracts => "contracts", 
			CurrencyType.Present1 => "present_1", 
			CurrencyType.Present2 => "present_2", 
			CurrencyType.Present3 => "present_3", 
			CurrencyType.Present4 => "present_4", 
			_ => throw new ArgumentOutOfRangeException("Wrong currency type: " + currencyType), 
		};
	}

	public static UnlockType ParseUnlockType(string key)
	{
		return key switch
		{
			"min_lvl" => UnlockType.MinLevel, 
			"max_lvl" => UnlockType.MaxLevel, 
			"enable_from" => UnlockType.EnableFrom, 
			"disable_from" => UnlockType.DisableFrom, 
			"girl_not_in_collection" => UnlockType.NotOwnedGirl, 
			"girl_in_collection" => UnlockType.OwnedGirl, 
			"lot_bought" => UnlockType.LotBought, 
			"lot_not_bought" => UnlockType.LotNotBought, 
			"level" => UnlockType.Level, 
			"task_complete" => UnlockType.TaskComplete, 
			"task_rewarded" => UnlockType.TaskRewarded, 
			"step_complete" => UnlockType.StepComplete, 
			"character_min_promote" => UnlockType.CharacterMinPromote, 
			"character_max_promote" => UnlockType.CharacterMaxPromote, 
			"dialogue_complete" => UnlockType.DialogueComplete, 
			"room_object" => UnlockType.RoomObject, 
			"tutor_id_complete" => UnlockType.TutorialStep, 
			"story_complete" => UnlockType.StoryComplete, 
			"resource_qty_max_star" => UnlockType.StarMax, 
			"step_not_rewarded" => UnlockType.StepNotRewarded, 
			"task_not_rewarded" => UnlockType.TaskNotRewarded, 
			"first_purchase" => UnlockType.FirstPurchase, 
			"min_payment_count" => UnlockType.MinPaymentCount, 
			"max_payment_count" => UnlockType.MaxPaymentCount, 
			"payment_count" => UnlockType.PaymentCount, 
			"min_purchases_in_section" => UnlockType.MinBoughtInSection, 
			"max_purchases_in_section" => UnlockType.MaxBoughtInSection, 
			"purchases_in_section" => UnlockType.BoughtInSection, 
			"dialogue_not_completed" => UnlockType.DialogueNotComplete, 
			"event_start" => UnlockType.EventStart, 
			"event_in_progress" => UnlockType.EventInProgress, 
			"event_rewarded" => UnlockType.EventRewarded, 
			"event_target" => UnlockType.EventTarget, 
			"enable_from_event" => UnlockType.EnableFromEvent, 
			"disable_from_event" => UnlockType.DisableFromEvent, 
			"bp_level" => UnlockType.BattlePassLevel, 
			"bp_level_range" => UnlockType.BattlePassLevelRange, 
			"bp_in_progress" => UnlockType.BattlePassInProgress, 
			"any_lot_bought" => UnlockType.AnyLotBought, 
			"event_target_range" => UnlockType.EventTargetRange, 
			"unreachable" => UnlockType.Unreachable, 
			"mini_event_in_progress" => UnlockType.MinieventInProgress, 
			"sum_price_average" => UnlockType.SumPriceAverage, 
			"roulette_roll_greater" => UnlockType.RouletteRollGreater, 
			"roulette_roll_less" => UnlockType.RouletteRollLess, 
			"subscription_active" => UnlockType.SubscriptionActive, 
			"subscription_not_active" => UnlockType.SubscriptionNotActive, 
			"roulette_bank_roll_greater" => UnlockType.RouletteBankRollGreater, 
			"roulette_bank_roll_less" => UnlockType.RouletteBankRollLess, 
			"merge_item" => UnlockType.MergeItem, 
			"summon_use" => UnlockType.SummonUse, 
			"spend_event_energy" => UnlockType.SpendEventEnergy, 
			"relationship_rewarded" => UnlockType.RelationshipRewarded, 
			_ => throw new ArgumentException("Unknown unlock type key: " + key), 
		};
	}
}
