using System;
using System.Linq;
using GreenT.Model.Collections;
using UniRx;

namespace GreenT.HornyScapes.Tutorial;

public class TutorialGroupManager : SimpleManager<TutorialGroupSteps>
{
	public TutorialGroupSteps GetGroup(int groupId)
	{
		return collection.FirstOrDefault((TutorialGroupSteps _group) => _group.GroupID == groupId);
	}

	public TutorialStep GetStep(int groupId, int stepId)
	{
		TutorialStep result = null;
		TutorialGroupSteps group = GetGroup(groupId);
		if (group != null)
		{
			result = group.Steps.Find((TutorialStep _step) => _step.StepID == stepId);
		}
		return result;
	}

	public IObservable<TutorialGroupSteps> GetUncompletedGroupObservable()
	{
		return Collection.Where((TutorialGroupSteps _group) => !_group.IsCompleted.Value).ToObservable().Merge(OnNew);
	}

	public void Initialize()
	{
		foreach (TutorialGroupSteps item in Collection)
		{
			item.Initialize();
		}
	}
}
