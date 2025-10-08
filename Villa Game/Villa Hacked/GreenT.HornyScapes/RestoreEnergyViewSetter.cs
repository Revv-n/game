using GreenT.HornyScapes.Bank.GoldenTickets.UI;
using Zenject;

namespace GreenT.HornyScapes;

public class RestoreEnergyViewSetter : IInitializable
{
	private GoldenTicketViewController goldenTicketController;

	private RestoreEnergyViewController restoreEnergyViewController;

	private SubscriptionGoldenTicketViewController _subscriptionGoldenTicketViewController;

	private GoldenTicketView goldenTicketView;

	private RestoreEnergyView restoreEnergyView;

	private SubscriptionGoldenTicketView _subscriptionGoldenTicketView;

	[Inject]
	public void Init(GoldenTicketViewController goldenTicketController, RestoreEnergyViewController restoreEnergyViewController, SubscriptionGoldenTicketViewController subscriptionViewController, SubscriptionGoldenTicketView subscriptionGoldenTicketView, GoldenTicketView goldenTicketView, RestoreEnergyView restoreEnergyView)
	{
		this.restoreEnergyView = restoreEnergyView;
		this.restoreEnergyViewController = restoreEnergyViewController;
		this.goldenTicketView = goldenTicketView;
		this.goldenTicketController = goldenTicketController;
		_subscriptionGoldenTicketView = subscriptionGoldenTicketView;
		_subscriptionGoldenTicketViewController = subscriptionViewController;
	}

	public void Initialize()
	{
		goldenTicketController.SetView(goldenTicketView);
		restoreEnergyViewController.SetView(restoreEnergyView);
		_subscriptionGoldenTicketViewController.SetView(_subscriptionGoldenTicketView);
	}
}
