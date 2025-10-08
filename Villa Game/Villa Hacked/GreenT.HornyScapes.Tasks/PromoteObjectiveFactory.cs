using System;
using System.Linq;
using System.Text.RegularExpressions;
using GreenT.Data;
using GreenT.HornyScapes.Card;
using GreenT.HornyScapes.Collections.Promote.UI;
using GreenT.HornyScapes.Tasks.Data;
using GreenT.Types;
using StripClub.Model;
using StripClub.Model.Cards;
using StripClub.Model.Shop;
using UnityEngine;

namespace GreenT.HornyScapes.Tasks;

public class PromoteObjectiveFactory : ConcreteObjectiveBaseFactory
{
	private const string PROMOTE_GIRL_RULE = "anyPromoteGirls";

	private const string PROMOTE_CONCRETE_GIRL_RULE = "promote:(\\d+):(\\d+)";

	private const string PROMOTE_CONCRETE_GIRL_COMPARER = "promote:";

	private const string PROMOTE_ANY_MAIN_GIRL_RULE = "promote_main";

	private const string PROMOTE_CONCRETE_EVENT_GIRL_RULE = "promote_event:(\\d+)";

	private const string PROMOTE_CONCRETE_EVENT_GIRL_COMPARER = "promote_event:";

	private const string PROMOTE_RARE_GIRL_RULE = "promote_rare";

	private const string PROMOTE_EPIC_GIRL_RULE = "promote_epic";

	private const string PROMOTE_LEGENDARY_GIRL_RULE = "promote_lega";

	private const string PROMOTE_CONCRETE_MAIN_GIRL_RULE = "promote_concmain:(\\d+)";

	private const string PROMOTE_CONCRETE_MAIN_GIRL_COMPARER = "promote_concmain:";

	private const string SPEND_SOFT_PROMOTE = "spendPromoteSoft";

	private readonly CardsCollectionTracker _cardsCollectionTracker;

	private readonly CardsCollection _cardsCollection;

	private readonly CalendarQueue _calendarQueue;

	private readonly PromoteNotifier _promoteNotifier;

	public PromoteObjectiveFactory(TaskObjectiveIcons objectiveIcons, GameSettings gameSettings, ICurrencyProcessor currencyProcessor, ISaver saver, CardsCollectionTracker cardsCollectionTracker, CardsCollection cardsCollection, CalendarQueue calendarQueue, PromoteNotifier promoteNotifier)
		: base(objectiveIcons, gameSettings, currencyProcessor, saver, "anyPromoteGirls|promote:(\\d+):(\\d+)|promote_main|promote_rare|promote_epic|promote_lega|promote_concmain:(\\d+)|promote_event:(\\d+)|spendPromoteSoft")
	{
		_cardsCollectionTracker = cardsCollectionTracker;
		_cardsCollection = cardsCollection;
		_calendarQueue = calendarQueue;
		_promoteNotifier = promoteNotifier;
	}

	public override IObjective Create(TaskMapper mapper, int index, ContentType contentType)
	{
		Match match = _objectiveRegex.Match(mapper.req_items[index]);
		try
		{
			if (match.Success)
			{
				switch (match.Value)
				{
				case "anyPromoteGirls":
					return CreateAnyGirlPromoteObjective(mapper, mapper.req_value[index], () => _objectiveIcons.AnyPromoteGirlObjective, contentType);
				case "promote_main":
					return CreateAnyMainGirlPromoteObjective(mapper, mapper.req_value[index], () => _objectiveIcons.AnyPromoteGirlObjective);
				case "promote_rare":
					return CreateConcreteRarityGirlPromoteObjective(mapper, mapper.req_value[index], () => _objectiveIcons.AnyPromoteGirlObjective, Rarity.Rare);
				case "promote_epic":
					return CreateConcreteRarityGirlPromoteObjective(mapper, mapper.req_value[index], () => _objectiveIcons.AnyPromoteGirlObjective, Rarity.Epic);
				case "promote_lega":
					return CreateConcreteRarityGirlPromoteObjective(mapper, mapper.req_value[index], () => _objectiveIcons.AnyPromoteGirlObjective, Rarity.Legendary);
				case "spendPromoteSoft":
					return CreateSpendForPromoteObjective(mapper, mapper.req_value[index], () => _objectiveIcons.AnyPromoteGirlObjective);
				}
			}
			if (match.Value.Contains("promote:"))
			{
				return CreateConcreteGirlPromoteObjective(mapper, mapper.req_value[index], () => _objectiveIcons.AnyPromoteGirlObjective, contentType);
			}
			if (match.Value.Contains("promote_concmain:"))
			{
				return CreateConcreteMainGirlPromoteObjective(mapper, mapper.req_value[index], () => _objectiveIcons.AnyPromoteGirlObjective);
			}
			if (match.Value.Contains("promote_event:"))
			{
				return CreateConcreteEventGirlPromoteObjective(mapper, mapper.req_value[index], () => _objectiveIcons.AnyPromoteGirlObjective);
			}
			return null;
		}
		catch (Exception innerException)
		{
			throw innerException.SendException($"{GetType().Name}: Can't create Objective {mapper.task_id}" + " from " + mapper.req_items[index]);
		}
	}

	private GainObjective CreateAnyGirlPromoteObjective(TaskMapper mapper, int reqValue, Func<Sprite> icon, ContentType contentType)
	{
		SavableObjectiveData savableObjectiveData = CreateGainData(mapper.task_id, reqValue);
		AnyGirlPromoteObjective result = new AnyGirlPromoteObjective(icon, savableObjectiveData, _cardsCollectionTracker, contentType);
		_saver.Add(savableObjectiveData);
		return result;
	}

	private GainObjective CreateAnyMainGirlPromoteObjective(TaskMapper mapper, int reqValue, Func<Sprite> icon)
	{
		SavableObjectiveData savableObjectiveData = CreateGainData(mapper.task_id, reqValue);
		AnyMainGirlPromoteObjective result = new AnyMainGirlPromoteObjective(icon, savableObjectiveData, _cardsCollectionTracker);
		_saver.Add(savableObjectiveData);
		return result;
	}

	private GainObjective CreateSpendForPromoteObjective(TaskMapper mapper, int reqValue, Func<Sprite> icon)
	{
		SavableObjectiveData savableObjectiveData = CreateGainData(mapper.task_id, reqValue);
		SpendForPromoteObjective result = new SpendForPromoteObjective(icon, CurrencyType.Soft, _promoteNotifier, savableObjectiveData);
		_saver.Add(savableObjectiveData);
		return result;
	}

	private GainObjective CreateConcreteRarityGirlPromoteObjective(TaskMapper mapper, int reqValue, Func<Sprite> icon, Rarity rarity)
	{
		SavableObjectiveData savableObjectiveData = CreateGainData(mapper.task_id, reqValue);
		ConcreteRarityGirlPromoteObjective result = new ConcreteRarityGirlPromoteObjective(icon, savableObjectiveData, _cardsCollectionTracker, rarity);
		_saver.Add(savableObjectiveData);
		return result;
	}

	private GainObjective CreateConcreteGirlPromoteObjective(TaskMapper mapper, int reqValue, Func<Sprite> icon, ContentType contentType)
	{
		SavableObjectiveData savableObjectiveData = CreateGainData(mapper.task_id, reqValue);
		Match match = Regex.Match(mapper.req_items.First(), "promote:(\\d+):(\\d+)");
		int girlId = 0;
		int targetLevel = 0;
		if (match.Success)
		{
			girlId = int.Parse(match.Groups[1].Value);
			targetLevel = int.Parse(match.Groups[2].Value);
		}
		ConcreteGirlPromoteObjective result = new ConcreteGirlPromoteObjective(icon, savableObjectiveData, _cardsCollectionTracker, girlId, targetLevel, contentType);
		_saver.Add(savableObjectiveData);
		return result;
	}

	private GainObjective CreateConcreteMainGirlPromoteObjective(TaskMapper mapper, int reqValue, Func<Sprite> icon)
	{
		SavableObjectiveData savableObjectiveData = CreateGainData(mapper.task_id, reqValue);
		Match match = Regex.Match(mapper.req_items.First(), "promote_concmain:(\\d+)");
		int girlId = 0;
		if (match.Success)
		{
			girlId = int.Parse(match.Groups[1].Value);
		}
		ConcreteMainGirlPromoteObjective result = new ConcreteMainGirlPromoteObjective(icon, savableObjectiveData, _cardsCollectionTracker, girlId);
		_saver.Add(savableObjectiveData);
		return result;
	}

	private GainObjective CreateConcreteEventGirlPromoteObjective(TaskMapper mapper, int reqValue, Func<Sprite> icon)
	{
		SavableObjectiveData savableObjectiveData = CreateGainData(mapper.task_id, reqValue);
		Match match = Regex.Match(mapper.req_items.First(), "promote_event:(\\d+)");
		int targetEventID = 0;
		if (match.Success)
		{
			targetEventID = int.Parse(match.Groups[1].Value);
		}
		ConcreteEventGirlPromoteObjective result = new ConcreteEventGirlPromoteObjective(icon, savableObjectiveData, _cardsCollectionTracker, _cardsCollection, targetEventID, _calendarQueue);
		_saver.Add(savableObjectiveData);
		return result;
	}
}
