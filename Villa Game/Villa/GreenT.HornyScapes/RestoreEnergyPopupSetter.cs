using GreenT.HornyScapes.Bank.GoldenTickets.UI;

namespace GreenT.HornyScapes;

public class RestoreEnergyPopupSetter
{
	private readonly GoldenTicketViewController _goldenTicketController;

	private readonly RestoreEnergyViewController _restoreEnergyViewController;

	private readonly SubscriptionGoldenTicketViewController _subscriptionGoldenTicketController;

	public RestoreEnergyPopupSetter(GoldenTicketViewController goldenTicketController, SubscriptionGoldenTicketViewController subscriptionGoldenTicketController, RestoreEnergyViewController restoreEnergyViewController)
	{
		_goldenTicketController = goldenTicketController;
		_restoreEnergyViewController = restoreEnergyViewController;
		_subscriptionGoldenTicketController = subscriptionGoldenTicketController;
	}

	public void Set(bool isTicketActive)
	{
		_goldenTicketController.Set(isTicketActive);
		_restoreEnergyViewController.Set(!isTicketActive);
	}

	public void SetSubscription(bool isSubscriptionAvailable)
	{
		_restoreEnergyViewController.Set(!isSubscriptionAvailable);
		_subscriptionGoldenTicketController.Set(isSubscriptionAvailable);
	}
}
