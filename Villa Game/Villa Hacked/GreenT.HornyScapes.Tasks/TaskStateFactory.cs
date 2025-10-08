using System.Collections.Generic;
using StripClub.Model.Quest;
using Zenject;

namespace GreenT.HornyScapes.Tasks;

public class TaskStateFactory : IFactory<Task, Dictionary<StateType, TaskStateBase>>, IFactory
{
	public Dictionary<StateType, TaskStateBase> Create(Task task)
	{
		return new Dictionary<StateType, TaskStateBase>
		{
			{
				StateType.Locked,
				CreateLockedState(task)
			},
			{
				StateType.Active,
				CreateActiveState(task)
			},
			{
				StateType.Complete,
				CreateCompleteState(task)
			},
			{
				StateType.Rewarded,
				CreateRewardState(task)
			}
		};
	}

	protected virtual TaskStateBase CreateLockedState(Task task)
	{
		return new TaskLockedState(task, StateType.Locked);
	}

	protected virtual TaskStateBase CreateActiveState(Task task)
	{
		return new TaskActiveState(task, StateType.Active);
	}

	protected virtual TaskStateBase CreateCompleteState(Task task)
	{
		return new TaskCompleteState(task, StateType.Complete);
	}

	protected virtual TaskStateBase CreateRewardState(Task task)
	{
		return new TaskRewardState(task, StateType.Rewarded);
	}
}
