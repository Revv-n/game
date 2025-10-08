using GreenT.HornyScapes.Tasks;
using StripClub.Model.Quest;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class MiniEventTaskStateFactory : TaskStateFactory
{
	protected override TaskStateBase CreateRewardState(Task task)
	{
		return new MiniEventTaskRewardState(task, StateType.Rewarded);
	}
}
