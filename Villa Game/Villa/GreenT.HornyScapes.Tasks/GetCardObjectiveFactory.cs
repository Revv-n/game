using System;
using System.Linq;
using System.Text.RegularExpressions;
using GreenT.Data;
using GreenT.HornyScapes.Card;
using GreenT.HornyScapes.Tasks.Data;
using GreenT.Types;
using StripClub.Model.Cards;
using StripClub.Model.Shop;
using UnityEngine;

namespace GreenT.HornyScapes.Tasks;

public class GetCardObjectiveFactory : ConcreteObjectiveBaseFactory
{
	private const string GET_GIRL_RULE = "get_cards_main";

	private const string GET_RARE_GIRL_RULE = "get_cards_rare";

	private const string GET_EPIC_GIRL_RULE = "get_cards_epic";

	private const string GET_LEGENDARY_GIRL_RULE = "get_cards_lega";

	private const string GET_CONCRETE_GIRL_RULE = "get_cards_concmain:(\\d+)";

	private const string GET_CONCRETE_GIRL_COMPARER = "get_cards_concmain:";

	private readonly CardsCollectionTracker _cardsCollectionTracker;

	public GetCardObjectiveFactory(TaskObjectiveIcons objectiveIcons, GameSettings gameSettings, ICurrencyProcessor currencyProcessor, ISaver saver, CardsCollectionTracker cardsCollectionTracker)
		: base(objectiveIcons, gameSettings, currencyProcessor, saver, "get_cards_main|get_cards_concmain:(\\d+)|get_cards_rare|get_cards_epic|get_cards_lega")
	{
		_cardsCollectionTracker = cardsCollectionTracker;
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
				case "get_cards_main":
					return CreateGetAnyGirlObjective(mapper, mapper.req_value[index], () => _objectiveIcons.GetCardObjective);
				case "get_cards_rare":
					return CreateGetConcreteRarityGirlObjective(mapper, mapper.req_value[index], () => _objectiveIcons.GetCardObjective, Rarity.Rare);
				case "get_cards_epic":
					return CreateGetConcreteRarityGirlObjective(mapper, mapper.req_value[index], () => _objectiveIcons.GetCardObjective, Rarity.Epic);
				case "get_cards_lega":
					return CreateGetConcreteRarityGirlObjective(mapper, mapper.req_value[index], () => _objectiveIcons.GetCardObjective, Rarity.Legendary);
				}
			}
			if (match.Value.Contains("get_cards_concmain:"))
			{
				return CreateGetConcreteGirlObjective(mapper, mapper.req_value[index], () => _objectiveIcons.GetCardObjective);
			}
			return null;
		}
		catch (Exception innerException)
		{
			throw innerException.SendException($"{GetType().Name}: Can't create Objective {mapper.task_id}" + " from " + mapper.req_items[index]);
		}
	}

	private GainObjective CreateGetAnyGirlObjective(TaskMapper mapper, int reqValue, Func<Sprite> icon)
	{
		SavableObjectiveData savableObjectiveData = CreateGainData(mapper.task_id, reqValue);
		AnyGetGirlObjective result = new AnyGetGirlObjective(icon, savableObjectiveData, _cardsCollectionTracker);
		_saver.Add(savableObjectiveData);
		return result;
	}

	private GainObjective CreateGetConcreteRarityGirlObjective(TaskMapper mapper, int reqValue, Func<Sprite> icon, Rarity rarity)
	{
		SavableObjectiveData savableObjectiveData = CreateGainData(mapper.task_id, reqValue);
		ConcreteRarityGetGirlObjective result = new ConcreteRarityGetGirlObjective(icon, savableObjectiveData, _cardsCollectionTracker, rarity);
		_saver.Add(savableObjectiveData);
		return result;
	}

	private GainObjective CreateGetConcreteGirlObjective(TaskMapper mapper, int reqValue, Func<Sprite> icon)
	{
		SavableObjectiveData savableObjectiveData = CreateGainData(mapper.task_id, reqValue);
		Match match = Regex.Match(mapper.req_items.First(), "get_cards_concmain:(\\d+)");
		int girlId = 0;
		if (match.Success)
		{
			girlId = int.Parse(match.Groups[1].Value);
		}
		ConcreteGetGirlObjective result = new ConcreteGetGirlObjective(icon, savableObjectiveData, _cardsCollectionTracker, girlId);
		_saver.Add(savableObjectiveData);
		return result;
	}
}
