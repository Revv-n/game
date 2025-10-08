using System;
using GreenT.HornyScapes.MergeCore;
using GreenT.HornyScapes.Tasks;
using Merge;
using Merge.MotionDesign;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Animations;

public class TaskMergeSystem : MonoBehaviour
{
	public StarToTaskIconTweenBuilder tweenBuilder;

	private Subject<GameItem> onItemCreated = new Subject<GameItem>();

	private CompositeDisposable trackStarStream = new CompositeDisposable();

	[Inject]
	private TaskController taskController;

	private IObservable<GameItem> OnItemCreated => onItemCreated.AsObservable();

	protected virtual void OnDestroy()
	{
		if ((bool)Controller<GameItemController>.Instance)
		{
			Controller<GameItemController>.Instance.OnItemCreated -= OnItemCreate;
			Controller<GameItemController>.Instance.OnItemTakenFromSomethere -= OnItemCreate;
		}
		trackStarStream.Dispose();
		onItemCreated.Dispose();
	}

	public void Start()
	{
		Controller<GameItemController>.Instance.OnItemCreated += OnItemCreate;
		Controller<GameItemController>.Instance.OnItemTakenFromSomethere += OnItemCreate;
		IObservable<Task> right = from _task in taskController.OnUpdate.OfType<Task, Task>()
			where _task.ReadyToComplete && _task.Goal.Objectives[0] is MergeItemObjective
			select _task;
		OnItemCreated.ZipLatest(right, (GameItem _createdItem, Task _task) => (_task: _task, _createdItem: _createdItem)).Where(HasGameItem).Subscribe(tweenBuilder.FlyStar, delegate(Exception ex)
		{
			throw ex.LogException();
		})
			.AddTo(trackStarStream);
		static bool HasGameItem((Task _task, GameItem _createdItem) _group)
		{
			return _group._createdItem != null;
		}
	}

	private void OnItemCreate(GameItem gameItem)
	{
		onItemCreated.OnNext(gameItem);
	}
}
