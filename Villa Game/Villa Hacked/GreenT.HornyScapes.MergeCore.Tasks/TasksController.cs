using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Events.Content;
using GreenT.HornyScapes.Tasks;
using GreenT.HornyScapes.Tasks.UI;
using GreenT.HornyScapes.UI;
using GreenT.Types;
using GreenT.UI;
using Merge;
using Merge.Core.Tasks;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.MergeCore.Tasks;

public class TasksController : Controller<TasksController>, Merge.IInputBlocker
{
	private TasksManager taskManager;

	private MainUiWindow mainWindow;

	private TaskBook window;

	private bool hasReadyToCompleteTask;

	private CompositeDisposable disposables = new CompositeDisposable();

	private TaskController taskController;

	private IWindowsManager windowsManager;

	private ContentSelectorGroup contentSelectorGroup;

	private TaskManagerCluster managerCluster;

	public bool BlocksClick => window.IsOpened;

	private IEnumerable<Task> activeTasks => taskManager.Tasks.Where((Task _task) => _task.IsActive);

	private GameItemController Field => Controller<GameItemController>.Instance;

	[Inject]
	private void InnerInit(IWindowsManager windowsManager, TaskController taskController, TaskManagerCluster managerCluster)
	{
		this.windowsManager = windowsManager;
		this.taskController = taskController;
		this.managerCluster = managerCluster;
		taskManager = managerCluster[ContentType.Main];
	}

	public void Set(TasksManager manager)
	{
		taskManager = manager;
		Init();
	}

	public override void Preload()
	{
		base.Preload();
		Field.OnItemCreated -= AtItemCreated;
		Field.OnItemRemoved -= AtItemRemoved;
		Field.OnItemTakenFromSomethere -= AtItemRemoved;
		Controller<LockedController>.Instance.OnItemActionUnlock -= AtItemCreated;
		window = windowsManager.Get<TaskBook>();
		mainWindow = windowsManager.Get<MainUiWindow>();
		Field.OnItemCreated += AtItemCreated;
		Field.OnItemRemoved += AtItemRemoved;
		Field.OnItemTakenFromSomethere += AtItemRemoved;
		Controller<LockedController>.Instance.OnItemActionUnlock += AtItemCreated;
	}

	public override void Init()
	{
		disposables.Clear();
		foreach (Task task in taskManager.Tasks)
		{
			RefreshAllMarks(task);
		}
	}

	private void RefreshAllMarks(Task task)
	{
		ValidateCompleteNotify();
	}

	private void AtItemCreated(GameItem item)
	{
		if (!IsItemFitForCompleteTask(item))
		{
			item.OnModuleRemoved += ModuleRemoved;
		}
		ValidateTasksProgress(item, GetItemsOnField(item.Key).Count);
		if (!hasReadyToCompleteTask)
		{
			ValidateCompleteNotify();
		}
		RefreshTaskMark(item);
	}

	private void ModuleRemoved(GameItem item, GIModuleType moduleType)
	{
		RefreshTaskMark(item);
	}

	private void RefreshTaskMark(GameItem item)
	{
		if (IsItemUsedInTasks(item))
		{
			MarkTaskItem(item, state: true);
		}
	}

	private void AtItemRemoved(GameItem item)
	{
		item.OnBlockActionChange -= RefreshTaskMark;
		ValidateTasksProgress(item, GetItemsOnField(item.Key).Count);
		if (hasReadyToCompleteTask)
		{
			ValidateCompleteNotify();
		}
	}

	private void MarkTaskItem(GameItem item, bool state)
	{
		if (item.HasTaskMark != state)
		{
			item.ChangeTaskMarkRef(state ? 1 : (-1));
		}
	}

	private void ShowExtraMark(GameItem item)
	{
		if (!item.HasLightSubTile)
		{
			Controller<TileMediator>.Instance.ExtraTaskSubscribe(item);
		}
	}

	private void HideExtraMark(GameItem item)
	{
		if (item.HasLightSubTile)
		{
			Controller<TileMediator>.Instance.ExtraTaskUnsubscribe(item);
		}
	}

	private void ShowAllExtraMark(Task task)
	{
	}

	private void HideAllTaskExtraMark(Task task)
	{
	}

	private void HideTaskExtraMark(Objective objective)
	{
		foreach (GameItem item in Field.FindItems(objective.ItemKey))
		{
			HideExtraMark(item);
		}
	}

	private void HideTaskMark(Objective objective)
	{
		foreach (GameItem item in Field.FindItems(objective.ItemKey))
		{
			MarkTaskItem(item, state: false);
		}
	}

	private void HideUnusedMark(Task task)
	{
	}

	private IEnumerable<Objective> GetItemsUsingInCompletedTask(Task task)
	{
		taskManager.Tasks.Where((Task _task) => _task.ReadyToComplete);
		return null;
	}

	private List<GameItem> GetItemsOnField(GIKey key)
	{
		return Field.FindAvailableItems(key);
	}

	private bool IsItemUsedInTasks(GameItem item)
	{
		IsItemFitForCompleteTask(item);
		return false;
	}

	private bool IsItemUsedInTasks(GIKey itemKey)
	{
		return false;
	}

	private bool IsItemFitForCompleteTask(GameItem gi)
	{
		if (!gi.IsLocked)
		{
			return !gi.IsBubbled;
		}
		return false;
	}

	private void ValidateMarkForAllTaskSubTile()
	{
		foreach (Task activeTask in activeTasks)
		{
			if (activeTask.ReadyToComplete)
			{
				ShowAllExtraMark(activeTask);
				continue;
			}
			HideAllTaskExtraMark(activeTask);
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Task>(Observable.Where<Task>(activeTask.OnUpdate, (Func<Task, bool>)((Task _item) => _item.ReadyToComplete)), (Action<Task>)ShowAllExtraMark), (Component)this);
		}
	}

	private void ValidateTasksProgress(GameItem changedItem, int currentCount)
	{
	}

	private void ValidateCompleteNotify()
	{
	}

	protected virtual void OnDisable()
	{
		disposables.Clear();
	}

	protected override void OnDestroy()
	{
		disposables.Dispose();
		base.OnDestroy();
	}
}
