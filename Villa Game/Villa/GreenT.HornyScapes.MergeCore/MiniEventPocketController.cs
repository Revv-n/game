using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.GameItems;
using GreenT.HornyScapes.MiniEvents;
using Merge;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.MergeCore;

public class MiniEventPocketController : MiniEventMergeController<MiniEventPocketController>
{
	private GameItemConfigManager _gameItemConfigManager;

	[Inject]
	public void Init(GameItemConfigManager gameItemConfigManager)
	{
		_gameItemConfigManager = gameItemConfigManager;
	}

	public override void Init()
	{
		base.Init();
		PocketController instance = Controller<PocketController>.Instance;
		List<GIData> newCollection = instance.PocketItemsQueue.Where((GIData item) => !IsMiniEventCurrencyItem(_gameItemConfigManager.GetConfigOrNull(item.Key))).ToList();
		instance.RebaseMain(newCollection);
		instance.OnItemAdd.Subscribe(AtItemAdded).AddTo(_trackStream);
		instance.OnItemRemove.Subscribe(AtItemRemoved).AddTo(_trackStream);
	}

	public override void RestoreItems(IEnumerable<GIData> items)
	{
		PocketController instance = Controller<PocketController>.Instance;
		foreach (GIData item in items)
		{
			instance.AddItemToQueue(item);
		}
	}

	private void AtItemAdded(GIData item)
	{
		GIConfig configOrNull = _gameItemConfigManager.GetConfigOrNull(item.Key);
		if (IsMiniEventCurrencyItem(configOrNull))
		{
			_mergeItemDispenser.Set(item, MiniEventGameItemLocation.Pocket);
		}
	}

	private void AtItemRemoved(GIData item)
	{
		GIConfig configOrNull = _gameItemConfigManager.GetConfigOrNull(item.Key);
		if (IsMiniEventCurrencyItem(configOrNull))
		{
			_mergeItemDispenser.Remove(item, MiniEventGameItemLocation.Pocket);
		}
	}
}
