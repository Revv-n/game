using GreenT.HornyScapes.MergeCore;
using StripClub.Model.Quest;

namespace GreenT.HornyScapes.Tasks;

public sealed class TaskRewardState : TaskStateBase
{
	public TaskRewardState(Task source, StateType state)
		: base(source, state)
	{
	}

	public override void Enter()
	{
		base.Enter();
		IObjective[] objectives = _source.Goal.Objectives;
		for (int i = 0; i < objectives.Length; i++)
		{
			objectives[i].OnRewardTask();
		}
		_source.Unsubscribe();
		Controller<SelectionController>.Instance.ClearSelection();
	}
}
