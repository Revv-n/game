using GreenT.HornyScapes.MergeCore;
using GreenT.Types;

namespace GreenT.HornyScapes.MiniEvents;

public class MiniEventMergeItemsDataCase : MiniEventItemsDataCase
{
	private const string Key = "mini.event.merge.save";

	public MiniEventMergeItemsDataCase()
	{
		_saveKey = "mini.event.merge.save";
	}

	public override void LoadItems(CompositeIdentificator id)
	{
		if (_items.TryGetValue(id, out var value))
		{
			Controller<MiniEventFieldController>.Instance.RestoreItems(value);
		}
	}
}
