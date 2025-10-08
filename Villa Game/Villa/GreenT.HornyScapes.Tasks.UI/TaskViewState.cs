using UnityEngine;

namespace GreenT.HornyScapes.Tasks.UI;

public class TaskViewState : MonoBehaviour, IState
{
	protected Task source;

	public void Set(Task task)
	{
		if (source != null)
		{
			Exit();
		}
		source = task;
	}

	public virtual void Enter()
	{
	}

	public virtual void Exit()
	{
	}
}
