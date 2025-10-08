using System;
using GreenT.HornyScapes.MergeStore;
using Merge;
using StripClub.Model.Quest;
using StripClub.UI;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Tasks.UI;

public class TaskView : MonoView<Task>, IDisposable
{
	[Header("Main")]
	public Transform MergeSubContainer;

	public Transform DescriptionSubContainer;

	[Header("Complete")]
	public Transform MergeSubContainerComplete;

	public Transform DescriptionSubContainerComplete;

	[Space]
	public UIObjectiveBlockSelector ObjectiveBlockSelector;

	public RewardTypeTask RewardType;

	public ButtonStrategy ButtonStrategy;

	public TaskViewStateMachine TaskViewStateMachine;

	[SerializeField]
	private TMP_Text _editorInfoText;

	private readonly Subject<Task> _onSet = new Subject<Task>();

	private readonly CompositeDisposable _taskUpdateStream = new CompositeDisposable();

	private ObjectiveViewManager _subItemViewManager;

	private ItemsCluster _mergeStoreItemsCluster;

	private readonly CompositeDisposable _mergeStoreStream = new CompositeDisposable();

	public bool IsInPool { get; set; }

	public IObservable<Task> OnSet => _onSet.AsObservable();

	[Inject]
	private void Init(ObjectiveViewManager manager, ItemsCluster itemsCluster)
	{
		_mergeStoreItemsCluster = itemsCluster;
		_subItemViewManager = manager;
	}

	public override void Set(Task task)
	{
		base.Set(task);
		IsInPool = false;
		TaskViewStateMachine.Init(task);
		ButtonStrategy.Set(task);
		SetView(task);
		TrackUpdate();
		TaskViewStateMachine.ForceSetFirstShowState();
		TaskViewStateMachine.SetState(base.Source.State);
		_onSet.OnNext(task);
	}

	private void SetView(Task task)
	{
		SetRewardType();
		ObjectiveBlockSelector.Set(task);
		ShowSubItems();
	}

	private void SetRewardType()
	{
		RewardType.Set(base.Source.Reward);
	}

	private void ShowSubItems()
	{
		_subItemViewManager.HideAll();
		_mergeStoreStream.Clear();
		CreatGoal(MergeSubContainer, DescriptionSubContainer, shoveInfo: true, useSale: true);
		CreatCompleteGoal();
	}

	private void CreatCompleteGoal()
	{
		CreatGoal(MergeSubContainerComplete, DescriptionSubContainerComplete);
	}

	private void CreatGoal(Transform mergeSubContainer, Transform descriptionSubContainer, bool shoveInfo = false, bool useSale = false)
	{
		IObjective[] objectives = base.Source.Goal.Objectives;
		foreach (IObjective objective in objectives)
		{
			ObjectiveView view = _subItemViewManager.Display(objective);
			bool position = false;
			MergeItemObjective mergeObjective = objective as MergeItemObjective;
			if (mergeObjective != null)
			{
				position = true;
				view.transform.SetParent(mergeSubContainer);
				if (useSale)
				{
					_mergeStoreItemsCluster.OnUpdate.Where(IsValid(mergeObjective.ItemKey)).Subscribe(delegate
					{
						UpdateMergeSale(view, mergeObjective);
					}).AddTo(_mergeStoreStream);
					UpdateMergeSale(view, mergeObjective);
				}
				else
				{
					view.SetIsSale(isSale: false);
				}
			}
			else
			{
				view.transform.SetParent(descriptionSubContainer);
				view.transform.localPosition = Vector3.zero;
				view.SetIsSale(isSale: false);
			}
			view.ChangeInfoButton(shoveInfo);
			view.SetPosition(position);
		}
	}

	private void UpdateMergeSale(ObjectiveView view, MergeItemObjective mergeObjective)
	{
		bool isSale = _mergeStoreItemsCluster.HaveLessOrEqual(base.Source.ContentType, mergeObjective.ItemKey);
		view.SetIsSale(isSale);
	}

	private static Func<GIKey, bool> IsValid(GIKey key)
	{
		return (GIKey giKey) => giKey.Collection == key.Collection && giKey.ID <= key.ID;
	}

	private void TrackUpdate()
	{
		_taskUpdateStream.Clear();
		base.Source.OnUpdate.TakeWhile((Task task) => task.State != StateType.Rewarded).Subscribe(delegate(Task task)
		{
			TaskViewStateMachine.SetState(task.State);
		}).AddTo(_taskUpdateStream);
	}

	protected virtual void OnDisable()
	{
		_taskUpdateStream.Clear();
		_mergeStoreStream.Clear();
	}

	public virtual void Dispose()
	{
		_mergeStoreStream.Dispose();
		_taskUpdateStream.Dispose();
	}
}
