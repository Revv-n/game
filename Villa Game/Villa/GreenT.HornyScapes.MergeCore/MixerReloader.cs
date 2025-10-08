using System.Linq;
using GreenT.HornyScapes.Card;
using Merge;

namespace GreenT.HornyScapes.MergeCore;

public class MixerReloader : ReloaderBase
{
	public MixerReloader(CardsCollectionTracker cardsCollectionTracker, MergeFieldProvider mergeFieldProvider)
		: base(cardsCollectionTracker, mergeFieldProvider)
	{
	}

	protected override void RefreshModules(GameItem item)
	{
		Controller<MixerController>.Instance.RefreshMixer(item.GetBox<GIBox.Mixer>());
	}

	protected override GameItem[] GetFieldModules()
	{
		return mergeField.Field.Objects.Where((GameItem _item) => _item.IsMixer).ToArray();
	}
}
