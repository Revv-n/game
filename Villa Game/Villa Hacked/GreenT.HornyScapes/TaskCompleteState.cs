using GreenT.HornyScapes.Tasks;
using StripClub.Model.Quest;

namespace GreenT.HornyScapes;

public sealed class TaskCompleteState : TaskStateBase
{
	public TaskCompleteState(Task source, StateType state)
		: base(source, state)
	{
	}
}
