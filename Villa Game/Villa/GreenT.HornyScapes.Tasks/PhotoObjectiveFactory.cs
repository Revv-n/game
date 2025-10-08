using System;
using System.Linq;
using System.Text.RegularExpressions;
using GreenT.Data;
using GreenT.HornyScapes.Messenger;
using GreenT.HornyScapes.Tasks.Data;
using GreenT.Types;
using StripClub.Model.Cards;
using StripClub.Model.Shop;
using UnityEngine;

namespace GreenT.HornyScapes.Tasks;

public class PhotoObjectiveFactory : ConcreteObjectiveBaseFactory
{
	private const string GET_PHOTO_RULE = "getPhoto";

	private const string GET_RARE_PHOTO_RULE = "getPhotoRare";

	private const string GET_EPIC_PHOTO_RULE = "getPhotoEpic";

	private const string GET_LEGENDARY_PHOTO_RULE = "getPhotoLega";

	private const string GET_CONCRETE_GIRL_PHOTO_RULE = "getPhoto_concmain:(\\d+)";

	private const string GET_CONCRETE_GIRL_PHOTO_COMPARER = "getPhoto_concmain:";

	private readonly DialoguesTracker _dialoguesTracker;

	public PhotoObjectiveFactory(TaskObjectiveIcons objectiveIcons, GameSettings gameSettings, ICurrencyProcessor currencyProcessor, ISaver saver, DialoguesTracker dialoguesTracker)
		: base(objectiveIcons, gameSettings, currencyProcessor, saver, "getPhotoRare|getPhotoEpic|getPhotoLega|getPhoto_concmain:(\\d+)|getPhoto")
	{
		_dialoguesTracker = dialoguesTracker;
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
				case "getPhotoRare":
					return TryCreateGetConcreteRarityPhotoObjectives(mapper, mapper.req_value[index], () => _objectiveIcons.GetCardObjective, Rarity.Rare);
				case "getPhotoEpic":
					return TryCreateGetConcreteRarityPhotoObjectives(mapper, mapper.req_value[index], () => _objectiveIcons.GetCardObjective, Rarity.Epic);
				case "getPhotoLega":
					return TryCreateGetConcreteRarityPhotoObjectives(mapper, mapper.req_value[index], () => _objectiveIcons.GetCardObjective, Rarity.Legendary);
				}
			}
			if (match.Value.Contains("getPhoto_concmain:"))
			{
				return TryCreateGetConcreteGirlPhotoObjectives(mapper, mapper.req_value[index], () => _objectiveIcons.GetCardObjective);
			}
			if (match.Value.Contains("getPhoto"))
			{
				return TryCreateGetPhotoObjectives(mapper, mapper.req_value[index], () => _objectiveIcons.GetPhotoObjective);
			}
			return null;
		}
		catch (Exception innerException)
		{
			throw innerException.SendException($"{GetType().Name}: Can't create Objective {mapper.task_id}" + " from " + mapper.req_items[index]);
		}
	}

	private GetPhotoObjective TryCreateGetPhotoObjectives(TaskMapper mapper, int reqValue, Func<Sprite> icon)
	{
		SavableObjectiveData savableObjectiveData = CreateGainData(mapper.task_id, reqValue);
		GetPhotoObjective result = new GetPhotoObjective(icon, savableObjectiveData, _dialoguesTracker);
		_saver.Add(savableObjectiveData);
		return result;
	}

	private GainObjective TryCreateGetConcreteRarityPhotoObjectives(TaskMapper mapper, int reqValue, Func<Sprite> icon, Rarity rarity)
	{
		SavableObjectiveData savableObjectiveData = CreateGainData(mapper.task_id, reqValue);
		GetConcreteRarityGirlPhotoObjective result = new GetConcreteRarityGirlPhotoObjective(icon, savableObjectiveData, _dialoguesTracker, rarity);
		_saver.Add(savableObjectiveData);
		return result;
	}

	private GainObjective TryCreateGetConcreteGirlPhotoObjectives(TaskMapper mapper, int reqValue, Func<Sprite> icon)
	{
		SavableObjectiveData savableObjectiveData = CreateGainData(mapper.task_id, reqValue);
		Match match = Regex.Match(mapper.req_items.First(), "getPhoto_concmain:(\\d+)");
		int girlId = 0;
		if (match.Success)
		{
			girlId = int.Parse(match.Groups[1].Value);
		}
		GetConcreteGirlPhotoObjective result = new GetConcreteGirlPhotoObjective(icon, savableObjectiveData, _dialoguesTracker, girlId);
		_saver.Add(savableObjectiveData);
		return result;
	}
}
