using GreenT.Types;

namespace GreenT.HornyScapes.MiniEvents;

public class MiniEventMergeItemsCleanService : MiniEventMergeItemsService
{
	public MiniEventMergeItemsCleanService(MiniEventMergeItemsDataCase mergeItemsDataCase, MiniEventInventoryItemsDataCase inventoryItemsDataCase, MiniEventPocketItemsDataCase pocketItemsDataCase)
		: base(mergeItemsDataCase, inventoryItemsDataCase, pocketItemsDataCase)
	{
	}

	public void Clean(CompositeIdentificator id)
	{
		_mergeItemsDataCase.Reset(id);
		_inventoryItemsDataCase.Reset(id);
		_pocketItemsDataCase.Reset(id);
	}
}
