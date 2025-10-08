using System.Collections.Generic;
using GreenT.Data;
using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.Lootboxes;
using GreenT.HornyScapes.Tasks.Data;
using GreenT.Types;
using StripClub.Model;
using StripClub.Model.Quest;
using Zenject;

namespace GreenT.HornyScapes.Tasks;

public class TaskFactory : AbstractTaskFactory<TaskMapper, Task>
{
	public TaskFactory(LockerFactory lockerFactory, LinkedContentFactory contentFactory, GoalFactory goalFactory, ISaver saver, LinkedContentAnalyticDataFactory analyticDataFactory, TaskContentTypeEnumConverter taskContentTypeEnumConverter, TaskStateFactory taskStateFactory)
		: base(lockerFactory, contentFactory, goalFactory, saver, analyticDataFactory, taskContentTypeEnumConverter, (IFactory<Task, Dictionary<StateType, TaskStateBase>>)taskStateFactory)
	{
	}

	public override Task CreateTask(TaskMapper mapper, IGoal goal, LinkedContent content, ILocker locker, ContentType contentType)
	{
		return new Task(mapper.task_id, goal, content, locker, taskContentTypeEnumConverter.Convert(mapper.content_type));
	}

	public override Goal CreateGoal(TaskMapper mapper, ContentType contentType)
	{
		return goalFactory.Create(mapper, contentType);
	}
}
