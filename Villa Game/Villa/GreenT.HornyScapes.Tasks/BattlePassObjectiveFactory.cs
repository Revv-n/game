using System;
using System.Text.RegularExpressions;
using GreenT.Data;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Tasks.Data;
using GreenT.Types;
using StripClub.Model.Shop;
using UnityEngine;

namespace GreenT.HornyScapes.Tasks;

public class BattlePassObjectiveFactory : ConcreteObjectiveBaseFactory
{
	private const string BP_LEVELS_RULE = "getLevels";

	private const string BP_REWARDS_RULE = "getRewards";

	private readonly BattlePassProvider _battlePassProvider;

	public BattlePassObjectiveFactory(TaskObjectiveIcons objectiveIcons, GameSettings gameSettings, ICurrencyProcessor currencyProcessor, ISaver saver, BattlePassProvider battlePassProvider)
		: base(objectiveIcons, gameSettings, currencyProcessor, saver, "getLevels|getRewards")
	{
		_battlePassProvider = battlePassProvider;
	}

	public override IObjective Create(TaskMapper mapper, int index, ContentType contentType)
	{
		Match match = _objectiveRegex.Match(mapper.req_items[index]);
		try
		{
			string value = match.Value;
			if (!(value == "getLevels"))
			{
				if (value == "getRewards")
				{
					return TryCreateBPRewardsObjectives(mapper, mapper.req_value[index], () => _objectiveIcons.BPRewardsObjective);
				}
				return null;
			}
			return TryCreateBPLevelsObjectives(mapper, mapper.req_value[index], () => _objectiveIcons.BPLevelsObjective);
		}
		catch (Exception innerException)
		{
			throw innerException.SendException($"{GetType().Name}: Can't create Objective {mapper.task_id}" + " from " + mapper.req_items[index]);
		}
	}

	private GainObjective TryCreateBPLevelsObjectives(TaskMapper mapper, int reqValue, Func<Sprite> icon)
	{
		SavableObjectiveData savableObjectiveData = CreateGainData(mapper.task_id, reqValue);
		GetBPLevelsObjective result = new GetBPLevelsObjective(icon, savableObjectiveData, _battlePassProvider);
		_saver.Add(savableObjectiveData);
		return result;
	}

	private GainObjective TryCreateBPRewardsObjectives(TaskMapper mapper, int reqValue, Func<Sprite> icon)
	{
		SavableObjectiveData savableObjectiveData = CreateGainData(mapper.task_id, reqValue);
		GetBPRewardsObjective result = new GetBPRewardsObjective(icon, savableObjectiveData, _battlePassProvider);
		_saver.Add(savableObjectiveData);
		return result;
	}
}
