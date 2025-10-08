using System.Collections.Generic;
using System.Linq;
using GreenT.Data;
using GreenT.HornyScapes.Bank;
using GreenT.HornyScapes.Events.Content;
using GreenT.Types;

namespace GreenT.HornyScapes.Events;

public class EventOfferCleaner : IDataCleaner
{
	private readonly OfferManagerCluster offerManagerCluster;

	private readonly ISaver saver;

	public EventOfferCleaner(OfferManagerCluster offerManagerCluster, ISaver saver)
	{
		this.offerManagerCluster = offerManagerCluster;
		this.saver = saver;
	}

	public void ClearData()
	{
		List<Memento> list = new List<Memento>();
		IEnumerable<OfferSettings> collection = offerManagerCluster[ContentType.Event].Collection;
		foreach (OfferSettings item in collection)
		{
			item.Initialize();
			list.Add(item.SaveState());
		}
		List<ISavableState> savableObjets = collection.Cast<ISavableState>().ToList();
		if (this.saver is Saver saver)
		{
			saver.LoadState(list, savableObjets);
		}
	}
}
