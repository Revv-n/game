namespace GreenT.HornyScapes.Bank.GoldenTickets.UI;

public class GoldenTicketViewController
{
	private AvailableGoldenTicketFinder availableGoldenTicketFinder;

	private GoldenTicketView goldenTicketView;

	public GoldenTicketViewController(AvailableGoldenTicketFinder availableGoldenTicketFinder)
	{
		this.availableGoldenTicketFinder = availableGoldenTicketFinder;
	}

	public void SetView(GoldenTicketView goldenTicketView)
	{
		this.goldenTicketView = goldenTicketView;
		goldenTicketView.SetupButton();
	}

	public void Set(bool isTicketActive)
	{
		if (isTicketActive)
		{
			goldenTicketView.Set(availableGoldenTicketFinder.AvailableGoldenTicket);
		}
		DisplayGoldenTicket(isTicketActive);
	}

	private void DisplayGoldenTicket(bool show)
	{
		if (show)
		{
			if (!goldenTicketView.IsActive())
			{
				goldenTicketView.Display(display: true);
			}
		}
		else if (goldenTicketView.IsActive())
		{
			goldenTicketView.Display(display: false);
		}
	}
}
