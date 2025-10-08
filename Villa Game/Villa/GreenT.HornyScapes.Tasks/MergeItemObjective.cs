using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.MergeCore;
using Merge;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Tasks;

[Objective]
public class MergeItemObjective : Objective<ObjectiveData>
{
	public List<GameItem> ProgressItems = new List<GameItem>();

	private readonly MergeIconService _iconProvider;

	private readonly CompositeDisposable _itemsTrack = new CompositeDisposable();

	public GIKey ItemKey { get; }

	public override bool IsComplete => ProgressItems.Count >= Data.Required;

	public MergeItemObjective(GIKey key, ObjectiveData data, MergeIconService iconProvider)
		: base(data)
	{
		_iconProvider = iconProvider;
		ItemKey = key;
	}

	public override Sprite GetIcon()
	{
		return _iconProvider.GetSprite(ItemKey);
	}

	public override int GetProgress()
	{
		return ProgressItems.Count;
	}

	public override int GetTarget()
	{
		return Data.Required;
	}

	public override void Track()
	{
		ProgressItems.Clear();
		GameItemController instance = Controller<GameItemController>.Instance;
		BubbleController instance2 = Controller<BubbleController>.Instance;
		LockedController instance3 = Controller<LockedController>.Instance;
		ProgressItems.AddRange(instance.FindAvailableItems(ItemKey));
		if (IsComplete)
		{
			foreach (GameItem progressItem in ProgressItems)
			{
				Controller<TileMediator>.Instance.ExtraTaskSubscribe(progressItem);
			}
		}
		foreach (GameItem progressItem2 in ProgressItems)
		{
			progressItem2.ChangeTaskMarkRef(1);
		}
		_itemsTrack.Clear();
		TrackItemsCreating(instance, instance2, instance3).Subscribe(OnItemCreated).AddTo(_itemsTrack);
		TrackItemsRemoving(instance).Subscribe(OnItemRemoved).AddTo(_itemsTrack);
		onUpdate.OnNext(this);
	}

	public override void OnRewardTask()
	{
		GameItemController instance = Controller<GameItemController>.Instance;
		foreach (GameItem progressItem in ProgressItems)
		{
			Controller<TileMediator>.Instance.ExtraTaskUnsubscribe(progressItem);
		}
		_itemsTrack.Clear();
		instance.PassMergeItem(ItemKey, Data.Required);
		foreach (GameItem progressItem2 in ProgressItems)
		{
			progressItem2.ChangeTaskMarkRef(-1);
		}
		ProgressItems.Clear();
	}

	private void OnItemCreated(GameItem item)
	{
		if (!(item.Key != ItemKey) && Controller<GameItemController>.Instance.IsAvailableItem(item))
		{
			item.ChangeTaskMarkRef(1);
			Controller<TileMediator>.Instance.ExtraTaskSubscribe(item);
			IncreaseProgress(item);
		}
	}

	private void IncreaseProgress(GameItem item)
	{
		ProgressItems.Add(item);
		onUpdate.OnNext(this);
	}

	private void OnItemRemoved(GameItem item)
	{
		if (!(item.Key != ItemKey) && Controller<GameItemController>.Instance.IsAvailableItem(item))
		{
			item.ChangeTaskMarkRef(-1);
			Controller<TileMediator>.Instance.ExtraTaskUnsubscribe(item);
			DecreaseProgress(item);
		}
	}

	private void DecreaseProgress(GameItem item)
	{
		ProgressItems.Remove(item);
		onUpdate.OnNext(this);
	}

	private IObservable<GameItem> TrackItemsCreating(GameItemController gic, BubbleController bubbleController, LockedController lockedController)
	{
		IObservable<GameItem> first = Observable.FromEvent(delegate(Action<GameItem> handler)
		{
			bubbleController.OnBubbleUnlock += handler;
		}, delegate(Action<GameItem> handler)
		{
			bubbleController.OnBubbleUnlock -= handler;
		});
		IObservable<GameItem> observable = Observable.FromEvent(delegate(Action<GameItem> handler)
		{
			lockedController.OnItemActionUnlock += handler;
		}, delegate(Action<GameItem> handler)
		{
			lockedController.OnItemActionUnlock -= handler;
		});
		IObservable<GameItem> observable2 = Observable.FromEvent(delegate(Action<GameItem> handler)
		{
			gic.OnItemCreated += handler;
		}, delegate(Action<GameItem> handler)
		{
			gic.OnItemCreated -= handler;
		});
		IObservable<GameItem> observable3 = Observable.FromEvent(delegate(Action<GameItem> handler)
		{
			gic.OnItemTakenFromSomethere += handler;
		}, delegate(Action<GameItem> handler)
		{
			gic.OnItemTakenFromSomethere -= handler;
		});
		return first.Merge(observable, observable2, observable3);
	}

	private IObservable<GameItem> TrackItemsRemoving(GameItemController gic)
	{
		return Observable.FromEvent(delegate(Action<GameItem> handler)
		{
			gic.OnItemRemoved += handler;
		}, delegate(Action<GameItem> handler)
		{
			gic.OnItemRemoved -= handler;
		});
	}
}
