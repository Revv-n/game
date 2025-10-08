using System.Linq;
using GreenT.HornyScapes.Constants;

namespace Merge.Core.Inventory;

public class InventorySettingsProvider
{
	private int _startOpenOpenSlotsCount;

	private int _closedSlotsCount;

	private int[] _pricesSlotsInventory;

	private const string StartOpentSlotsKey = "start_open_inventory";

	private const string ClosedSlotsKey = "closed_slots_inventory";

	private const string PricesSlotsKey = "prices_slots_inventory";

	public int StartOpenSlotsCount => _startOpenOpenSlotsCount;

	public int ClosedSlotsCount => _closedSlotsCount;

	public InventorySettingsProvider(Constants constants)
	{
		SetStartParameters(constants, constants);
	}

	public int GetSlotUnlockPrice(int slot)
	{
		return _pricesSlotsInventory[slot];
	}

	private void SetStartParameters(IConstants<int> intConstants, IConstants<string> stringConstants)
	{
		_startOpenOpenSlotsCount = intConstants["start_open_inventory"];
		_closedSlotsCount = intConstants["closed_slots_inventory"];
		string text = stringConstants["prices_slots_inventory"];
		_pricesSlotsInventory = (from s in text.Split(',')
			select int.TryParse(s, out var result) ? result : 0).ToArray();
	}
}
