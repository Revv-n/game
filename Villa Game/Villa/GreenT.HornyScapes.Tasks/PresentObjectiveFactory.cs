using System;
using System.Text.RegularExpressions;
using GreenT.Data;
using GreenT.HornyScapes.Presents.Services;
using GreenT.HornyScapes.Tasks.Data;
using GreenT.Types;
using StripClub.Model.Shop;
using UnityEngine;

namespace GreenT.HornyScapes.Tasks;

public class PresentObjectiveFactory : ConcreteObjectiveBaseFactory
{
	private const string ANY_PRESENT_ANY_GIRL_RULE = "givePresents";

	private readonly PresentsNotifier _presentsNotifier;

	public PresentObjectiveFactory(TaskObjectiveIcons objectiveIcons, GameSettings gameSettings, ICurrencyProcessor currencyProcessor, ISaver saver, PresentsNotifier presentsNotifier)
		: base(objectiveIcons, gameSettings, currencyProcessor, saver, "givePresents")
	{
		_presentsNotifier = presentsNotifier;
	}

	public override IObjective Create(TaskMapper mapper, int index, ContentType contentType)
	{
		Match match = _objectiveRegex.Match(mapper.req_items[index]);
		try
		{
			if (match.Success && match.Value == "givePresents")
			{
				return CreateAnyGirlAnyPresentObjective(mapper, mapper.req_value[index], () => _objectiveIcons.GetCardObjective);
			}
			return null;
		}
		catch (Exception innerException)
		{
			throw innerException.SendException($"{GetType().Name}: Can't create Objective {mapper.task_id}" + " from " + mapper.req_items[index]);
		}
	}

	private GainObjective CreateAnyGirlAnyPresentObjective(TaskMapper mapper, int reqValue, Func<Sprite> icon)
	{
		SavableObjectiveData savableObjectiveData = CreateGainData(mapper.task_id, reqValue);
		AnyGirlAnyPresentObjective result = new AnyGirlAnyPresentObjective(icon, savableObjectiveData, _presentsNotifier);
		_saver.Add(savableObjectiveData);
		return result;
	}
}
