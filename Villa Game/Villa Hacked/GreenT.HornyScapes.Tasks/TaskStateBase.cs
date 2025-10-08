using StripClub.Model.Quest;

namespace GreenT.HornyScapes.Tasks;

public class TaskStateBase
{
	protected Task _source;

	public StateType State { get; protected set; }

	public TaskStateBase(Task source, StateType state)
	{
		_source = source;
		State = state;
	}

	public virtual void Enter()
	{
	}

	public virtual void Exit()
	{
	}
}
