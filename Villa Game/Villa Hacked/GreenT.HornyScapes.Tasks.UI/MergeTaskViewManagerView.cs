using System;
using System.Collections.Generic;
using System.Linq;
using StripClub.UI;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Tasks.UI;

public class MergeTaskViewManagerView : MonoView<TasksManager>, IDisposable
{
	private const int SkipUpdateCount = 10;

	private readonly CompositeDisposable _taskListenerStream = new CompositeDisposable();

	private readonly CompositeDisposable _onUpdateTaskStateStream = new CompositeDisposable();

	private MergeTaskViewManager _viewManager;

	[Inject]
	private void InnerInit(MergeTaskViewManager viewManager)
	{
		_viewManager = viewManager;
	}

	private void SubscribeOnTaskUpdate()
	{
		_onUpdateTaskStateStream.Clear();
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<IGoal>(Observable.SelectMany<IGoal, IGoal>(Observable.Merge<IGoal>(base.Source.Tasks.Select((Task task) => task.Goal.OnUpdate)), (Func<IGoal, IObservable<IGoal>>)((IGoal updatedGoal) => Observable.Select<long, IGoal>(Observable.LastOrDefault<long>(Observable.Take<long>(Observable.EveryUpdate(), 10)), (Func<long, IGoal>)((long _) => updatedGoal)))), (Action<IGoal>)delegate
		{
			UpdateTasks();
		}), (ICollection<IDisposable>)_onUpdateTaskStateStream);
		foreach (Task task in base.Source.Tasks.Where((Task x) => !x.Lock.IsOpen.Value && !x.IsRewarded))
		{
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<bool>(Observable.Where<bool>((IObservable<bool>)task.Lock.IsOpen, (Func<bool, bool>)((bool _isOpen) => _isOpen)), (Action<bool>)delegate
			{
				ShowTaskTab(task);
			}), (ICollection<IDisposable>)_onUpdateTaskStateStream);
		}
	}

	public override void Set(TasksManager source)
	{
		base.Set(source);
		SubscribeOnTaskUpdate();
		if (base.isActiveAndEnabled)
		{
			Initialize();
		}
	}

	public void Initialize()
	{
		_viewManager.HideAll();
		Task[] array = base.Source.Tasks.Where((Task _task) => _task.IsActive).ToArray();
		foreach (Task task in array)
		{
			ShowTaskTab(task);
		}
	}

	private void ShowTaskTab(Task task)
	{
		TaskView taskView = _viewManager.GetView();
		taskView.Set(task);
		if (!task.ReadyToComplete)
		{
			_viewManager.MoveToLast(taskView);
		}
		else
		{
			_viewManager.MoveToFirst(taskView);
		}
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Task>(Observable.First<Task>(Observable.Where<Task>(task.OnUpdate, (Func<Task, bool>)((Task _task) => _task.IsRewarded && taskView.Source == _task && taskView.IsInPool))), (Action<Task>)delegate
		{
			taskView.Display(display: false);
		}, (Action<Exception>)delegate(Exception ex)
		{
			ex.LogException();
		}), (ICollection<IDisposable>)_taskListenerStream);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Task>(Observable.First<Task>(Observable.Where<Task>(task.OnUpdate, (Func<Task, bool>)((Task _task) => _task.ReadyToComplete && taskView.Source == _task))), (Action<Task>)delegate
		{
			UpdateTasks();
		}, (Action<Exception>)delegate(Exception ex)
		{
			ex.LogException();
		}), (ICollection<IDisposable>)_taskListenerStream);
		UpdateTasks();
	}

	private void UpdateTasks()
	{
		_viewManager.MoveReadyTasksUp();
	}

	protected virtual void OnDestroy()
	{
		Dispose();
	}

	public void Dispose()
	{
		CompositeDisposable taskListenerStream = _taskListenerStream;
		if (taskListenerStream != null)
		{
			taskListenerStream.Dispose();
		}
		CompositeDisposable onUpdateTaskStateStream = _onUpdateTaskStateStream;
		if (onUpdateTaskStateStream != null)
		{
			onUpdateTaskStateStream.Dispose();
		}
	}
}
