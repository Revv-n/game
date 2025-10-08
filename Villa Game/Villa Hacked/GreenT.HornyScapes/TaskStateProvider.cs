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
		return Observable.Where<Task>(Observable.Merge<Task>(Observable.SelectMany<Task, Task>(Observable.ToObservable<Task>(_cluster[contentType].Collection.Where((Task t) => !t.IsRewarded)), (Func<Task, IObservable<Task>>)((Task task) => task.OnUpdate)), Array.Empty<IObservable<Task>>()), (Func<Task, bool>)((Task task) => task.IsRewarded));
	}

	public IObservable<Task> OnRewardedAll()
	{
		return Observable.Where<Task>(Observable.Merge<Task>(Observable.SelectMany<Task, Task>(Observable.ToObservable<Task>(from t in GetAll()
			where !t.IsRewarded
			select t), (Func<Task, IObservable<Task>>)((Task task) => task.OnUpdate)), Array.Empty<IObservable<Task>>()), (Func<Task, bool>)((Task task) => task.IsRewarded));
	}
}
