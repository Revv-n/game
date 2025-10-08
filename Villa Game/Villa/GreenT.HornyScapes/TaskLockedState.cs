using GreenT.HornyScapes.Tasks;
using StripClub.Model.Quest;

namespace GreenT.HornyScapes;

public sealed class TaskLockedState : TaskStateBase
{
	public TaskLockedState(Task source, StateType state)
		: base(source, state)
	{
	}
}
