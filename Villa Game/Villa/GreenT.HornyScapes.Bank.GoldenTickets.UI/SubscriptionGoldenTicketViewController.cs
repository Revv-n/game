using System.Linq;
using StripClub.Model.Shop;

namespace GreenT.HornyScapes.Bank.GoldenTickets.UI;

public class SubscriptionGoldenTicketViewController
{
	private readonly LotManager _lotManager;

	private SubscriptionGoldenTicketView _view;

	public SubscriptionGoldenTicketViewController(LotManager lotManager)
	{
		_lotManager = lotManager;
	}

	public void SetView(SubscriptionGoldenTicketView view)
	{
		_view = view;
	}

	public void Set(bool isActive)
	{
		SubscriptionLot subscriptionLot = _lotManager.GetLot<SubscriptionLot>().FirstOrDefault((SubscriptionLot item) => item.Locker.IsOpen.Value);
		if (subscriptionLot != null)
		{
			_view.Set(subscriptionLot);
		}
		_view.Display(isActive && subscriptionLot != null);
	}
}
