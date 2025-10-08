using GreenT.HornyScapes.Tasks;
using StripClub.Model.Quest;

namespace GreenT.HornyScapes.MiniEvents;

public class MiniEventTaskRewardState : TaskStateBase
{
	public MiniEventTaskRewardState(Task source, StateType state)
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
	}
}
