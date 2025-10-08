namespace GreenT.HornyScapes.MiniEvents;

public class MiniEventMergeItemsService
{
	protected readonly MiniEventMergeItemsDataCase _mergeItemsDataCase;

	protected readonly MiniEventInventoryItemsDataCase _inventoryItemsDataCase;

	protected readonly MiniEventPocketItemsDataCase _pocketItemsDataCase;

	public MiniEventMergeItemsService(MiniEventMergeItemsDataCase mergeItemsDataCase, MiniEventInventoryItemsDataCase inventoryItemsDataCase, MiniEventPocketItemsDataCase pocketItemsDataCase)
	{
		_mergeItemsDataCase = mergeItemsDataCase;
		_inventoryItemsDataCase = inventoryItemsDataCase;
		_pocketItemsDataCase = pocketItemsDataCase;
	}
}
