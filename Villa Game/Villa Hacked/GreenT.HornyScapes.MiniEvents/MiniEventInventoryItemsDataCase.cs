using GreenT.HornyScapes.MergeCore;
using GreenT.Types;

namespace GreenT.HornyScapes.MiniEvents;

public class MiniEventInventoryItemsDataCase : MiniEventItemsDataCase
{
	private const string Key = "mini.event.inventory.save";

	public MiniEventInventoryItemsDataCase()
	{
		_saveKey = "mini.event.inventory.save";
	}

	public override void LoadItems(CompositeIdentificator id)
	{
		if (_items.TryGetValue(id, out var value))
		{
			Controller<MiniEventInventoryController>.Instance.RestoreItems(value);
		}
	}
}
