using StripClub;
using StripClub.Model.Shop;

namespace GreenT.HornyScapes.Events;

public class BankDataCleaner : CollectionManager<Lot>, IDataCleaner
{
	public void ClearData()
	{
		foreach (Lot item in collection)
		{
			item.Initialize();
		}
	}
}
