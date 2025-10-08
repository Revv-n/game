using System;
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
		base.Source.Tasks.Select((Task task) => task.Goal.OnUpdate).Merge().SelectMany((IGoal updatedGoal) => from _ in Observable.EveryUpdate().Take(10).LastOrDefault()
			select updatedGoal)
			.Subscribe(delegate
			{
				UpdateTasks();
			})
			.AddTo(_onUpdateTaskStateStream);
		foreach (Task task in base.Source.Tasks.Where((Task x) => !x.Lock.IsOpen.Value && !x.IsRewarded))
		{
			task.Lock.IsOpen.Where((bool _isOpen) => _isOpen).Subscribe(delegate
			{
				ShowTaskTab(task);
			}).AddTo(_onUpdateTaskStateStream);
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
		task.OnUpdate.Where((Task _task) => _task.IsRewarded && taskView.Source == _task && taskView.IsInPool).First().Subscribe(delegate
		{
			taskView.Display(display: false);
		}, delegate(Exception ex)
		{
			ex.LogException();
		})
			.AddTo(_taskListenerStream);
		task.OnUpdate.Where((Task _task) => _task.ReadyToComplete && taskView.Source == _task).First().Subscribe(delegate
		{
			UpdateTasks();
		}, delegate(Exception ex)
		{
			ex.LogException();
		})
			.AddTo(_taskListenerStream);
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
		_taskListenerStream?.Dispose();
		_onUpdateTaskStateStream?.Dispose();
	}
}
