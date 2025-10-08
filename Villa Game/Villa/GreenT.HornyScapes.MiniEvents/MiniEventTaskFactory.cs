using System.Collections.Generic;
using GreenT.Data;
using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.Lootboxes;
using GreenT.HornyScapes.Tasks;
using GreenT.Types;
using StripClub.Model;
using StripClub.Model.Quest;
using Zenject;

namespace GreenT.HornyScapes.MiniEvents;

public class MiniEventTaskFactory : AbstractTaskFactory<TaskActivityMapper, MiniEventTask>
{
	public MiniEventTaskFactory(LockerFactory lockerFactory, LinkedContentFactory contentFactory, GoalFactory goalFactory, ISaver saver, LinkedContentAnalyticDataFactory analyticDataFactory, TaskContentTypeEnumConverter taskContentTypeEnumConverter, MiniEventTaskStateFactory miniEventTaskStateFactory)
		: base(lockerFactory, contentFactory, goalFactory, saver, analyticDataFactory, taskContentTypeEnumConverter, (IFactory<Task, Dictionary<StateType, TaskStateBase>>)miniEventTaskStateFactory)
	{
	}

	public override MiniEventTask CreateTask(TaskActivityMapper mapper, IGoal goal, LinkedContent content, ILocker locker, ContentType contentType)
	{
		return new MiniEventTask(mapper, goal, content, locker, contentType);
	}

	public override Goal CreateGoal(TaskActivityMapper mapper, ContentType contentType)
	{
		return goalFactory.Create(mapper, contentType);
	}
}
