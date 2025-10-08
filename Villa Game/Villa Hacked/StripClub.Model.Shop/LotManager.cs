using System.Collections.Generic;
using System.Linq;
using GreenT.Model.Collections;

namespace StripClub.Model.Shop;

public class LotManager : SimpleManager<Lot>
{
	public IEnumerable<T> GetLot<T>() where T : Lot
	{
		return collection.OfType<T>();
	}

	public bool HasLotWithMonetizationID(int monetizationID)
	{
		return collection.Any((Lot _lot) => _lot.MonetizationID == monetizationID);
	}

	public Lot GetLotByMonetizationID(int monetizationID)
	{
		return Collection.FirstOrDefault((Lot _lot) => _lot.MonetizationID == monetizationID);
	}

	public void Initialize()
	{
		foreach (Lot item in Collection)
		{
			item.Initialize();
		}
	}
}
