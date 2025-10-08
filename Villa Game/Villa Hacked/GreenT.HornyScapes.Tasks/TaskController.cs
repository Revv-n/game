using System;
using System.Collections.Generic;
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

	public IObservable<Task> OnUpdate => Observable.AsObservable<Task>((IObservable<Task>)onUpdate);

	public bool HasAnyTaskReadyCompleted()
	{
		return manager.Tasks.Any((Task x) => x.State == StateType.Complete);
	}

	public TaskController(GameStarter gameStarter)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
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
		generalStream = ObservableExtensions.Subscribe<bool>((IObservable<bool>)gameStarter.IsGameActive, (Action<bool>)Track);
	}

	private void Track(bool track)
	{
		trackStream.Clear();
		if (track)
		{
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Task>(Observable.SelectMany<Task, Task>(Observable.ToObservable<Task>(manager.Tasks.Where((Task _task) => !_task.IsRewarded)), (Func<Task, IObservable<Task>>)EmitOnTaskUnlock), (Action<Task>)UnlockTask), (ICollection<IDisposable>)trackStream);
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Task>(Observable.SelectMany<Task, Task>(Observable.ToObservable<Task>(manager.Tasks), (Func<Task, IObservable<Task>>)((Task _task) => _task.OnUpdate)), (Action<Task>)onUpdate.OnNext), (ICollection<IDisposable>)trackStream);
		}
	}

	private IObservable<Task> EmitOnTaskUnlock(Task task)
	{
		return Observable.Select<bool, Task>(Observable.Where<bool>((IObservable<bool>)task.Lock.IsOpen, (Func<bool, bool>)((bool _isOpen) => task.IsLocked && _isOpen)), (Func<bool, Task>)((bool _) => task));
	}

	private void UnlockTask(Task task)
	{
		task.SelectState(StateType.Active);
	}

	public void Dispose()
	{
		onUpdate?.Dispose();
		CompositeDisposable obj = trackStream;
		if (obj != null)
		{
			obj.Dispose();
		}
		generalStream?.Dispose();
	}
}
