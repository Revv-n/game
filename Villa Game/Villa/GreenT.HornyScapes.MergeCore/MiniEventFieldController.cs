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
		Observable.FromEvent(delegate(Action<GameItem> handler)
		{
			gameItemContorller.OnItemCreated += handler;
		}, delegate(Action<GameItem> handler)
		{
			gameItemContorller.OnItemCreated -= handler;
		}).Subscribe(OnItemCreated).AddTo(_trackStream);
		Observable.FromEvent(delegate(Action<GameItem> handler)
		{
			gameItemContorller.OnItemRemoved += handler;
		}, delegate(Action<GameItem> handler)
		{
			gameItemContorller.OnItemRemoved -= handler;
		}).Subscribe(OnItemRemoved).AddTo(_trackStream);
		Observable.FromEvent(delegate(Action<GameItem> handler)
		{
			gameItemContorller.OnItemTakenFromSomethere += handler;
		}, delegate(Action<GameItem> handler)
		{
			gameItemContorller.OnItemTakenFromSomethere -= handler;
		}).Subscribe(OnItemRemoved).AddTo(_trackStream);
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
