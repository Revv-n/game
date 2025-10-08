using System.Linq;
using GreenT.HornyScapes.Sellouts.Models;
using GreenT.Model.Collections;

namespace GreenT.HornyScapes.Sellouts.Providers;

public class SelloutManager : SimpleManager<Sellout>
{
	public Sellout GetSellout(int id)
	{
		return Collection.FirstOrDefault((Sellout x) => x.ID == id);
	}

	public bool TryGetSellout(int id, out Sellout sellout)
	{
		sellout = GetSellout(id);
		return sellout != null;
	}

	public void RemoveSellout(Sellout sellout)
	{
		collection.Remove(sellout);
	}
}
