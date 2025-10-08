using System;
using System.Collections.Generic;
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

	private IObservable<GameItem> OnItemCreated => Observable.AsObservable<GameItem>((IObservable<GameItem>)onItemCreated);

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
		IObservable<Task> observable = Observable.Where<Task>(Observable.OfType<Task, Task>(taskController.OnUpdate), (Func<Task, bool>)((Task _task) => _task.ReadyToComplete && _task.Goal.Objectives[0] is MergeItemObjective));
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<(Task, GameItem)>(Observable.Where<(Task, GameItem)>(Observable.ZipLatest<GameItem, Task, (Task, GameItem)>(OnItemCreated, observable, (Func<GameItem, Task, (Task, GameItem)>)((GameItem _createdItem, Task _task) => (_task: _task, _createdItem: _createdItem))), (Func<(Task, GameItem), bool>)HasGameItem), (Action<(Task, GameItem)>)tweenBuilder.FlyStar, (Action<Exception>)delegate(Exception ex)
		{
			throw ex.LogException();
		}), (ICollection<IDisposable>)trackStarStream);
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
