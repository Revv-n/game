using System;
using System.Linq;
using System.Text.RegularExpressions;
using GreenT.Data;
using GreenT.HornyScapes.Tasks.Data;
using GreenT.Types;
using StripClub.Model.Shop;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Tasks;

public sealed class RouletteObjectiveFactory : ConcreteObjectiveBaseFactory
{
	private const string CONCRETE_ROULETTE_AMOUNT_RULE = "roulette:(\\d+)";

	private const string CONCRETE_ROULETTE_AMOUNT_COMPARER = "roulette:";

	private readonly SignalBus _signalBus;

	public RouletteObjectiveFactory(TaskObjectiveIcons objectiveIcons, GameSettings gameSettings, ICurrencyProcessor currencyProcessor, ISaver saver, SignalBus signalBus)
		: base(objectiveIcons, gameSettings, currencyProcessor, saver, "roulette:(\\d+)")
	{
		_signalBus = signalBus;
	}

	public override IObjective Create(TaskMapper mapper, int index, ContentType contentType)
	{
		Match match = _objectiveRegex.Match(mapper.req_items[index]);
		try
		{
			if (match.Value.Contains("roulette:"))
			{
				return CreateConcreteRouletteObjective(mapper, mapper.req_value[index], () => _objectiveIcons.AnyPromoteGirlObjective);
			}
			return null;
		}
		catch (Exception innerException)
		{
			throw innerException.SendException($"{GetType().Name}: Can't create Objective {mapper.task_id}" + " from " + mapper.req_items[index]);
		}
	}

	private GainObjective CreateConcreteRouletteObjective(TaskMapper mapper, int reqValue, Func<Sprite> icon)
	{
		SavableObjectiveData savableObjectiveData = CreateGainData(mapper.task_id, reqValue);
		Match match = Regex.Match(mapper.req_items.First(), "roulette:(\\d+)");
		int rouletteId = 0;
		if (match.Success)
		{
			rouletteId = int.Parse(match.Groups[1].Value);
		}
		ConcreteRouletteObjective result = new ConcreteRouletteObjective(icon, savableObjectiveData, _signalBus, rouletteId);
		_saver.Add(savableObjectiveData);
		return result;
	}
}
