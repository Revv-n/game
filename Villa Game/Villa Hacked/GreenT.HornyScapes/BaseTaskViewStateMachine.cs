using GreenT.HornyScapes.Tasks;
using GreenT.HornyScapes.Tasks.UI;
using StripClub.Model.Quest;

namespace GreenT.HornyScapes;

public class BaseTaskViewStateMachine<TRewardState> : SimpleStateMachine<StateType, TaskViewState> where TRewardState : TaskViewState
{
	public TaskViewState FirstShowState;

	public TaskViewState InProgressState;

	public TaskViewState CompleteState;

	public TRewardState RewardState;

	public void Init(Task task)
	{
		FirstShowState.Set(task);
		InProgressState.Set(task);
		CompleteState.Set(task);
		RewardState.Set(task);
	}

	public override void SetState(StateType stateType)
	{
		switch (stateType)
		{
		case StateType.Active:
		case StateType.Locked:
			SetState(InProgressState);
			break;
		case StateType.Complete:
			SetState(CompleteState);
			break;
		case StateType.Rewarded:
			SetState(RewardState);
			break;
		}
	}

	public void ForceSetFirstShowState()
	{
		SetState(FirstShowState);
	}

	public void ForceSetRewardState()
	{
		SetState(RewardState);
	}
}
