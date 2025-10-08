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
		_tasks = tasks;
		_saver = saver;
	}

	public void Initialize()
	{
		StartTrack();
	}

	public void Dispose()
	{
		_trackStream?.Dispose();
	}

	private void StartTrack()
	{
		_trackStream.Clear();
		_tasks.ToObservable().SelectMany((Func<MiniEventTask, IObservable<Task>>)EmitOnTaskUnlock).Subscribe(UnlockTask)
			.AddTo(_trackStream);
	}

	private IObservable<Task> EmitOnTaskUnlock(Task task)
	{
		return from _isOpen in task.Lock.IsOpen
			where task.IsLocked && _isOpen
			select _isOpen into _
			where task.State != StateType.Rewarded
			select task;
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
		_trackStream?.Clear();
	}
}
