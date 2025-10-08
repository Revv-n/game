using System;
using System.Text.RegularExpressions;
using GreenT.Data;
using GreenT.HornyScapes.Tasks.Data;
using GreenT.Types;
using StripClub.Model.Shop;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Tasks;

public class SummonObjectiveFactory : ConcreteObjectiveBaseFactory
{
	private const string ANY_SUMMON_RULE = "anySummon";

	private const string CONCRETE_SUMMON_AMOUNT_RULE = "getSummon";

	private readonly SignalBus _signalBus;

	public SummonObjectiveFactory(TaskObjectiveIcons objectiveIcons, GameSettings gameSettings, ICurrencyProcessor currencyProcessor, ISaver saver, SignalBus signalBus)
		: base(objectiveIcons, gameSettings, currencyProcessor, saver, "anySummon|getSummon")
	{
		_signalBus = signalBus;
	}

	public override IObjective Create(TaskMapper mapper, int index, ContentType contentType)
	{
		Match match = _objectiveRegex.Match(mapper.req_items[index]);
		try
		{
			if (match.Success)
			{
				string value = match.Value;
				if (value == "anySummon")
				{
					return CreateAnySummonObjective(mapper, mapper.req_value[index], () => _objectiveIcons.AnySummonObjective, contentType);
				}
				if (value == "getSummon")
				{
					return CreateConcreteSummonAmountObjective(mapper, mapper.req_value[index], () => _objectiveIcons.AnySummonObjective);
				}
			}
			return null;
		}
		catch (Exception innerException)
		{
			throw innerException.SendException($"{GetType().Name}: Can't create Objective {mapper.task_id}" + " from " + mapper.req_items[index]);
		}
	}

	private GainObjective CreateAnySummonObjective(TaskMapper mapper, int reqValue, Func<Sprite> icon, ContentType contentType)
	{
		SavableObjectiveData savableObjectiveData = CreateGainData(mapper.task_id, reqValue);
		AnySummonObjective result = new AnySummonObjective(icon, savableObjectiveData, _signalBus, contentType);
		_saver.Add(savableObjectiveData);
		return result;
	}

	private GainObjective CreateConcreteSummonAmountObjective(TaskMapper mapper, int reqValue, Func<Sprite> icon)
	{
		SavableObjectiveData savableObjectiveData = CreateGainData(mapper.task_id, reqValue);
		ConcreteSummonAmountObjective result = new ConcreteSummonAmountObjective(icon, savableObjectiveData, _signalBus);
		_saver.Add(savableObjectiveData);
		return result;
	}
}
