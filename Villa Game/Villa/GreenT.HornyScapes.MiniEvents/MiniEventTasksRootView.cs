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
		_taskListenerStream?.Dispose();
		_onUpdateTaskStateStream?.Dispose();
	}

	public override void Set(IEnumerable<MiniEventTask> sources)
	{
		base.Set(sources);
		_taskListenerStream?.Clear();
		_onUpdateTaskStateStream?.Clear();
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
			task.Lock.IsOpen.Where((bool _isOpen) => _isOpen).Subscribe(delegate
			{
				ShowTaskTab(task);
			}).AddTo(_onUpdateTaskStateStream);
		}
		_taskStateProvider.OnRewardedAll().Subscribe(delegate
		{
			UpdateTasks();
		}).AddTo(_onUpdateTaskStateStream);
	}

	private void ShowTaskTab(MiniEventTask task)
	{
		MiniEventTaskItemView taskView = _taskItemViewManager.Display(task);
		taskView.Set(task);
		_ = task.ReadyToComplete;
		task.OnUpdate.Where((Task _task) => _task.IsRewarded && taskView.Source == _task).First().Subscribe(delegate
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
	}
}
