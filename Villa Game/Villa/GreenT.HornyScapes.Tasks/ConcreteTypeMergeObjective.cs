using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.MergeCore;
using Merge;

namespace GreenT.HornyScapes.Tasks;

[Objective]
public class ConcreteTypeMergeObjective : OneWayMergeItemObjective
{
	public ConcreteTypeMergeObjective(GIKey key, SavableObjectiveData data, MergeIconService iconProvider)
		: base(key, data, iconProvider)
	{
	}

	protected override void OnItemCreated(GameItem item)
	{
		if (!(item.Key.Collection != base.ItemKey.Collection) && item.Key.ID > 1 && Controller<GameItemController>.Instance.IsAvailableItem(item))
		{
			IncreaseProgress(item);
		}
	}
}
