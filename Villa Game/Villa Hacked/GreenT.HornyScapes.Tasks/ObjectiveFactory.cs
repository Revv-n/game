using System.Collections.Generic;
using GreenT.HornyScapes.Tasks.Data;
using GreenT.Types;
using Zenject;

namespace GreenT.HornyScapes.Tasks;

public class ObjectiveFactory : IFactory<TaskMapper, ContentType, IObjective[]>, IFactory
{
	private readonly List<ConcreteObjectiveBaseFactory> _objectiveFactories;

	public ObjectiveFactory(CurrencyObjectiveFactory currencyObjectiveFactory, SummonObjectiveFactory summonObjectiveFactory, PromoteObjectiveFactory promoteObjectiveFactory, GetCardObjectiveFactory getCardObjectiveFactory, PhotoObjectiveFactory photoObjectiveFactory, BattlePassObjectiveFactory battlePassObjectiveFactory, PresentObjectiveFactory presentObjectiveFactory, MergeObjectiveFactory mergeObjectiveFactory, RouletteObjectiveFactory rouletteObjectiveFactory, CompletedDialogueObjectiveFactory completedDialogueObjectiveFactory, CompletedDialogueAnswersObjectiveFactory completedDialogueAnswersObjectiveFactory)
	{
		_objectiveFactories = new List<ConcreteObjectiveBaseFactory>
		{
			currencyObjectiveFactory, summonObjectiveFactory, promoteObjectiveFactory, getCardObjectiveFactory, photoObjectiveFactory, completedDialogueObjectiveFactory, completedDialogueAnswersObjectiveFactory, battlePassObjectiveFactory, rouletteObjectiveFactory, presentObjectiveFactory,
			mergeObjectiveFactory
		};
	}

	public IObjective[] Create(TaskMapper mapper, ContentType contentType)
	{
		IObjective[] array = new IObjective[mapper.req_items.Length];
		for (int i = 0; i < mapper.req_items.Length; i++)
		{
			for (int j = 0; j < _objectiveFactories.Count; j++)
			{
				IObjective objective = _objectiveFactories[j].Create(mapper, i, contentType);
				if (objective != null)
				{
					array[i] = objective;
					break;
				}
			}
		}
		return array;
	}
}
