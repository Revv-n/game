using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.MiniEvents;
using GreenT.HornyScapes.Tasks;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Saves;

public class MinieventTaskCompleteSaveEvent : SaveEvent
{
	private MiniEventTaskManager _miniEventTaskManager;

	[Inject]
	public void Init(MiniEventTaskManager miniEventTaskManager)
	{
		_miniEventTaskManager = miniEventTaskManager;
	}

	public override void Track()
	{
		Initialize();
	}

	private void Initialize()
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Task>(Observable.Where<Task>(Observable.Merge<Task>(Observable.SelectMany<MiniEventTask, Task>(Observable.ToObservable<MiniEventTask>(_miniEventTaskManager.Collection.Where((MiniEventTask t) => !t.IsRewarded)), (Func<MiniEventTask, IObservable<Task>>)((MiniEventTask task) => task.OnUpdate)), Array.Empty<IObservable<Task>>()), (Func<Task, bool>)((Task task) => task.IsRewarded)), (Action<Task>)delegate
		{
			Save();
		}), (ICollection<IDisposable>)saveStreams);
	}
}
