using GreenT.UI;
using StripClub.Model.Shop.Data;

namespace GreenT.HornyScapes.Bank.UI;

public class SubscriptionOfferWindow : Window
{
	public void OnLotBought(LotBoughtSignal signal)
	{
		if (!signal.Lot.IsAvailable() && IsOpened)
		{
			Close();
		}
	}
}
