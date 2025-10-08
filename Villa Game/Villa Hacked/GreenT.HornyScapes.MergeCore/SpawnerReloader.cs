using System.Linq;
using GreenT.HornyScapes.Card;
using Merge;

namespace GreenT.HornyScapes.MergeCore;

public class SpawnerReloader : ReloaderBase
{
	public SpawnerReloader(CardsCollectionTracker cardsCollectionTracker, MergeFieldProvider mergeFieldProvider)
		: base(cardsCollectionTracker, mergeFieldProvider)
	{
	}

	protected override void RefreshModules(GameItem item)
	{
		Controller<ClickSpawnController>.Instance.RefreshSpawner(item.GetBox<GIBox.ClickSpawn>());
	}

	protected override GameItem[] GetFieldModules()
	{
		return mergeField.Field.Objects.Where((GameItem _item) => _item.IsSpawner).ToArray();
	}
}
