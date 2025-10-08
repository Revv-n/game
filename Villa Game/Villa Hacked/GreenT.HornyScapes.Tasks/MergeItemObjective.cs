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
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
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
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<GameItem>(TrackItemsCreating(instance, instance2, instance3), (Action<GameItem>)OnItemCreated), (ICollection<IDisposable>)_itemsTrack);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<GameItem>(TrackItemsRemoving(instance), (Action<GameItem>)OnItemRemoved), (ICollection<IDisposable>)_itemsTrack);
		onUpdate.OnNext((IObjective)this);
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
		onUpdate.OnNext((IObjective)this);
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
		onUpdate.OnNext((IObjective)this);
	}

	private IObservable<GameItem> TrackItemsCreating(GameItemController gic, BubbleController bubbleController, LockedController lockedController)
	{
		IObservable<GameItem> observable = Observable.FromEvent<GameItem>((Action<Action<GameItem>>)delegate(Action<GameItem> handler)
		{
			bubbleController.OnBubbleUnlock += handler;
		}, (Action<Action<GameItem>>)delegate(Action<GameItem> handler)
		{
			bubbleController.OnBubbleUnlock -= handler;
		});
		IObservable<GameItem> observable2 = Observable.FromEvent<GameItem>((Action<Action<GameItem>>)delegate(Action<GameItem> handler)
		{
			lockedController.OnItemActionUnlock += handler;
		}, (Action<Action<GameItem>>)delegate(Action<GameItem> handler)
		{
			lockedController.OnItemActionUnlock -= handler;
		});
		IObservable<GameItem> observable3 = Observable.FromEvent<GameItem>((Action<Action<GameItem>>)delegate(Action<GameItem> handler)
		{
			gic.OnItemCreated += handler;
		}, (Action<Action<GameItem>>)delegate(Action<GameItem> handler)
		{
			gic.OnItemCreated -= handler;
		});
		IObservable<GameItem> observable4 = Observable.FromEvent<GameItem>((Action<Action<GameItem>>)delegate(Action<GameItem> handler)
		{
			gic.OnItemTakenFromSomethere += handler;
		}, (Action<Action<GameItem>>)delegate(Action<GameItem> handler)
		{
			gic.OnItemTakenFromSomethere -= handler;
		});
		return Observable.Merge<GameItem>(observable, new IObservable<GameItem>[3] { observable2, observable3, observable4 });
	}

	private IObservable<GameItem> TrackItemsRemoving(GameItemController gic)
	{
		return Observable.FromEvent<GameItem>((Action<Action<GameItem>>)delegate(Action<GameItem> handler)
		{
			gic.OnItemRemoved += handler;
		}, (Action<Action<GameItem>>)delegate(Action<GameItem> handler)
		{
			gic.OnItemRemoved -= handler;
		});
	}
}
