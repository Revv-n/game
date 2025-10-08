using System;
using System.Collections.Generic;
using GreenT.Data;
using GreenT.HornyScapes.Tasks;
using StripClub.Model.Quest;
using UniRx;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class MiniEventTasksController : IController, IDisposable
{
	private readonly IEnumerable<MiniEventTask> _tasks;

	private readonly ISaver _saver;

	private readonly CompositeDisposable _trackStream = new CompositeDisposable();

	public MiniEventTasksController(IEnumerable<MiniEventTask> tasks, ISaver saver)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		_tasks = tasks;
		_saver = saver;
	}

	public void Initialize()
	{
		StartTrack();
	}

	public void Dispose()
	{
		CompositeDisposable trackStream = _trackStream;
		if (trackStream != null)
		{
			trackStream.Dispose();
		}
	}

	private void StartTrack()
	{
		_trackStream.Clear();
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Task>(Observable.SelectMany<MiniEventTask, Task>(Observable.ToObservable<MiniEventTask>(_tasks), (Func<MiniEventTask, IObservable<Task>>)EmitOnTaskUnlock), (Action<Task>)UnlockTask), (ICollection<IDisposable>)_trackStream);
	}

	private IObservable<Task> EmitOnTaskUnlock(Task task)
	{
		return Observable.Select<bool, Task>(Observable.Where<bool>(Observable.Where<bool>((IObservable<bool>)task.Lock.IsOpen, (Func<bool, bool>)((bool _isOpen) => task.IsLocked && _isOpen)), (Func<bool, bool>)((bool _) => task.State != StateType.Rewarded)), (Func<bool, Task>)((bool _) => task));
	}

	private void UnlockTask(Task task)
	{
		task.SelectState(StateType.Active);
	}

	public void RefreshSavables()
	{
		foreach (MiniEventTask task in _tasks)
		{
			task.Initialize();
			task.SelectState(StateType.Active);
			IObjective[] objectives = task.Goal.Objectives;
			for (int i = 0; i < objectives.Length; i++)
			{
				if (objectives[i] is SavableObjective savableObjective)
				{
					_saver.Delete(savableObjective.Data);
				}
			}
		}
		CompositeDisposable trackStream = _trackStream;
		if (trackStream != null)
		{
			trackStream.Clear();
		}
	}
}
