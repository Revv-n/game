using GreenT.HornyScapes.MergeCore;
using GreenT.Types;

namespace GreenT.HornyScapes.MiniEvents;

public class MiniEventPocketItemsDataCase : MiniEventItemsDataCase
{
	private const string Key = "mini.event.pocket.save";

	public MiniEventPocketItemsDataCase()
	{
		_saveKey = "mini.event.pocket.save";
	}

	public override void LoadItems(CompositeIdentificator id)
	{
		if (_items.TryGetValue(id, out var value))
		{
			Controller<MiniEventPocketController>.Instance.RestoreItems(value);
		}
	}
}
