using System;
using System.Linq;
using StripClub.Model.Quest;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Tasks;

public class TaskController : IDisposable, IInitializable
{
	private Subject<Task> onUpdate = new Subject<Task>();

	private TasksManager manager;

	private CompositeDisposable trackStream = new CompositeDisposable();

	private IDisposable generalStream;

	private GameStarter gameStarter;

	public IObservable<Task> OnUpdate => onUpdate.AsObservable();

	public bool HasAnyTaskReadyCompleted()
	{
		return manager.Tasks.Any((Task x) => x.State == StateType.Complete);
	}

	public TaskController(GameStarter gameStarter)
	{
		this.gameStarter = gameStarter;
	}

	public void Set(TasksManager manager)
	{
		this.manager = manager;
		Track(gameStarter.IsGameActive.Value);
	}

	public void Initialize()
	{
		generalStream?.Dispose();
		generalStream = gameStarter.IsGameActive.Subscribe(Track);
	}

	private void Track(bool track)
	{
		trackStream.Clear();
		if (track)
		{
			manager.Tasks.Where((Task _task) => !_task.IsRewarded).ToObservable().SelectMany((Func<Task, IObservable<Task>>)EmitOnTaskUnlock)
				.Subscribe(UnlockTask)
				.AddTo(trackStream);
			manager.Tasks.ToObservable().SelectMany((Task _task) => _task.OnUpdate).Subscribe(onUpdate.OnNext)
				.AddTo(trackStream);
		}
	}

	private IObservable<Task> EmitOnTaskUnlock(Task task)
	{
		return from _isOpen in task.Lock.IsOpen
			where task.IsLocked && _isOpen
			select _isOpen into _
			select task;
	}

	private void UnlockTask(Task task)
	{
		task.SelectState(StateType.Active);
	}

	public void Dispose()
	{
		onUpdate?.Dispose();
		trackStream?.Dispose();
		generalStream?.Dispose();
	}
}
