using GreenT.HornyScapes.Tasks;
using StripClub.Model.Quest;

namespace GreenT.HornyScapes;

public sealed class TaskActiveState : TaskStateBase
{
	public TaskActiveState(Task source, StateType state)
		: base(source, state)
	{
	}

	public override void Enter()
	{
		base.Enter();
		IObjective[] objectives = _source.Goal.Objectives;
		for (int i = 0; i < objectives.Length; i++)
		{
			objectives[i].Track();
		}
		_source.UpdateTask();
	}
}
