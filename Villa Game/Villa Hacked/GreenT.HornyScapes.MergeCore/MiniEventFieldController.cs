using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.MiniEvents;
using GreenT.Types;
using Merge;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.MergeCore;

public class MiniEventFieldController : MiniEventMergeController<MiniEventFieldController>
{
	private MergeFieldProvider _mergeFieldProvider;

	private GameItemDistributor _gameItemDistributor;

	[Inject]
	private void InnerInit(MergeFieldProvider mergeFieldProvider, GameItemDistributor gameItemDistributor)
	{
		_mergeFieldProvider = mergeFieldProvider;
		_gameItemDistributor = gameItemDistributor;
	}

	public override void Init()
	{
		base.Init();
		GameItemController gameItemContorller = Controller<GameItemController>.Instance;
		if (_mergeFieldProvider.TryGetData(ContentType.Main, out var field))
		{
			foreach (GameItem item in field.Field.Objects.Where((GameItem gameItem) => IsMiniEventCurrencyItem(gameItem.Config)))
			{
				gameItemContorller.RemoveItem(item);
			}
		}
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<GameItem>(Observable.FromEvent<GameItem>((Action<Action<GameItem>>)delegate(Action<GameItem> handler)
		{
			gameItemContorller.OnItemCreated += handler;
		}, (Action<Action<GameItem>>)delegate(Action<GameItem> handler)
		{
			gameItemContorller.OnItemCreated -= handler;
		}), (Action<GameItem>)OnItemCreated), (ICollection<IDisposable>)_trackStream);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<GameItem>(Observable.FromEvent<GameItem>((Action<Action<GameItem>>)delegate(Action<GameItem> handler)
		{
			gameItemContorller.OnItemRemoved += handler;
		}, (Action<Action<GameItem>>)delegate(Action<GameItem> handler)
		{
			gameItemContorller.OnItemRemoved -= handler;
		}), (Action<GameItem>)OnItemRemoved), (ICollection<IDisposable>)_trackStream);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<GameItem>(Observable.FromEvent<GameItem>((Action<Action<GameItem>>)delegate(Action<GameItem> handler)
		{
			gameItemContorller.OnItemTakenFromSomethere += handler;
		}, (Action<Action<GameItem>>)delegate(Action<GameItem> handler)
		{
			gameItemContorller.OnItemTakenFromSomethere -= handler;
		}), (Action<GameItem>)OnItemRemoved), (ICollection<IDisposable>)_trackStream);
	}

	public override void RestoreItems(IEnumerable<GIData> items)
	{
		_mergeFieldProvider.TryGetData(ContentType.Main, out var field);
		PocketController instance = Controller<PocketController>.Instance;
		GameItemController instance2 = Controller<GameItemController>.Instance;
		foreach (GIData item in items)
		{
			if (!instance2.TryGetFirstEmptyPoint(out var _))
			{
				instance.AddItemToQueue(item);
				break;
			}
			_gameItemDistributor.AddItem(item, field);
		}
	}

	private void OnItemCreated(GameItem gameItem)
	{
		if (IsMiniEventCurrencyItem(gameItem.Config))
		{
			_mergeItemDispenser.Set(gameItem.Data, MiniEventGameItemLocation.Field);
		}
	}

	private void OnItemRemoved(GameItem gameItem)
	{
		if (IsMiniEventCurrencyItem(gameItem.Config))
		{
			_mergeItemDispenser.Remove(gameItem.Data, MiniEventGameItemLocation.Field);
		}
	}
}
