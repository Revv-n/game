using System;
using System.Linq;
using System.Text.RegularExpressions;
using GreenT.Data;
using GreenT.HornyScapes.Messenger;
using GreenT.HornyScapes.Tasks.Data;
using GreenT.Types;
using StripClub.Model.Shop;
using UnityEngine;

namespace GreenT.HornyScapes.Tasks;

public class CompletedDialogueObjectiveFactory : ConcreteObjectiveBaseFactory
{
	private const string COMPLETED_DIALOGUE_RULE_RULE = "сompletedDialogue_concmain:(\\d+)";

	private const string COMPLETED_DIALOGUE_RULE_COMPARER = "сompletedDialogue_concmain:";

	private readonly DialoguesTracker _dialoguesTracker;

	public CompletedDialogueObjectiveFactory(TaskObjectiveIcons objectiveIcons, GameSettings gameSettings, ICurrencyProcessor currencyProcessor, ISaver saver, DialoguesTracker dialoguesTracker)
		: base(objectiveIcons, gameSettings, currencyProcessor, saver, "сompletedDialogue_concmain:(\\d+)")
	{
		_dialoguesTracker = dialoguesTracker;
	}

	public override IObjective Create(TaskMapper mapper, int index, ContentType contentType)
	{
		Match match = _objectiveRegex.Match(mapper.req_items[index]);
		try
		{
			if (match.Value.Contains("сompletedDialogue_concmain:"))
			{
				return TryCreateGetConcreteCompletedDialogueObjectiveObjectives(mapper, mapper.req_value[index], () => _objectiveIcons.CompletedDialogueObjective);
			}
			return null;
		}
		catch (Exception innerException)
		{
			throw innerException.SendException($"{GetType().Name}: Can't create Objective {mapper.task_id}" + " from " + mapper.req_items[index]);
		}
	}

	private GainObjective TryCreateGetConcreteCompletedDialogueObjectiveObjectives(TaskMapper mapper, int reqValue, Func<Sprite> icon)
	{
		SavableObjectiveData savableObjectiveData = CreateGainData(mapper.task_id, reqValue);
		Match match = Regex.Match(mapper.req_items.First(), "сompletedDialogue_concmain:(\\d+)");
		int conversationId = 0;
		if (match.Success)
		{
			conversationId = int.Parse(match.Groups[1].Value);
		}
		GetConcreteCompletedDialogueObjective result = new GetConcreteCompletedDialogueObjective(icon, savableObjectiveData, _dialoguesTracker, conversationId);
		_saver.Add(savableObjectiveData);
		return result;
	}
}
