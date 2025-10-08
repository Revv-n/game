using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Tasks;
using GreenT.Types;
using UniRx;

namespace GreenT.HornyScapes;

public sealed class TaskStateProvider : ClusterCombiner<Task, TasksManager>
{
	public TaskStateProvider(IDictionary<ContentType, TasksManager> cluster)
		: base(cluster)
	{
	}

	public IObservable<Task> OnRewarded(ContentType contentType)
	{
		return from task in _cluster[contentType].Collection.Where((Task t) => !t.IsRewarded).ToObservable().SelectMany((Task task) => task.OnUpdate)
				.Merge()
			where task.IsRewarded
			select task;
	}

	public IObservable<Task> OnRewardedAll()
	{
		return from task in (from t in GetAll()
				where !t.IsRewarded
				select t).ToObservable().SelectMany((Task task) => task.OnUpdate).Merge()
			where task.IsRewarded
			select task;
	}
}
