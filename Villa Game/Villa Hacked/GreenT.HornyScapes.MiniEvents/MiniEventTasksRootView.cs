using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Tasks;
using StripClub.UI;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.MiniEvents;

public class MiniEventTasksRootView : MonoView<IEnumerable<MiniEventTask>>
{
	private CompositeDisposable _taskListenerStream;

	private CompositeDisposable _onUpdateTaskStateStream;

	private MiniEventTaskItemViewManager _taskItemViewManager;

	private TaskStateProvider _taskStateProvider;

	[Inject]
	private void Init(MiniEventTaskItemViewManager taskItemViewManager, TaskStateProvider taskStateProvider)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Expected O, but got Unknown
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected O, but got Unknown
		_taskItemViewManager = taskItemViewManager;
		_taskStateProvider = taskStateProvider;
		_taskListenerStream = new CompositeDisposable();
		_onUpdateTaskStateStream = new CompositeDisposable();
	}

	private void OnDestroy()
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

	public override void Set(IEnumerable<MiniEventTask> sources)
	{
		base.Set(sources);
		CompositeDisposable taskListenerStream = _taskListenerStream;
		if (taskListenerStream != null)
		{
			taskListenerStream.Clear();
		}
		CompositeDisposable onUpdateTaskStateStream = _onUpdateTaskStateStream;
		if (onUpdateTaskStateStream != null)
		{
			onUpdateTaskStateStream.Clear();
		}
		SubscribeOnTaskUpdate();
		Initialize();
	}

	private void Initialize()
	{
		_taskItemViewManager.HideAll();
		MiniEventTask[] array = base.Source.Where((MiniEventTask _task) => _task.IsActive).ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			ShowTaskTab(array[i]);
		}
	}

	private void SubscribeOnTaskUpdate()
	{
		_onUpdateTaskStateStream.Clear();
		foreach (MiniEventTask task in base.Source.Where((MiniEventTask x) => !x.Lock.IsOpen.Value && !x.IsRewarded))
		{
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<bool>(Observable.Where<bool>((IObservable<bool>)task.Lock.IsOpen, (Func<bool, bool>)((bool _isOpen) => _isOpen)), (Action<bool>)delegate
			{
				ShowTaskTab(task);
			}), (ICollection<IDisposable>)_onUpdateTaskStateStream);
		}
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Task>(_taskStateProvider.OnRewardedAll(), (Action<Task>)delegate
		{
			UpdateTasks();
		}), (ICollection<IDisposable>)_onUpdateTaskStateStream);
	}

	private void ShowTaskTab(MiniEventTask task)
	{
		MiniEventTaskItemView taskView = _taskItemViewManager.Display(task);
		taskView.Set(task);
		_ = task.ReadyToComplete;
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Task>(Observable.First<Task>(Observable.Where<Task>(task.OnUpdate, (Func<Task, bool>)((Task _task) => _task.IsRewarded && taskView.Source == _task))), (Action<Task>)delegate
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
	}
}
