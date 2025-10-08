using System.Collections.Generic;
using System.Linq;

namespace GreenT.HornyScapes.Bank.GoldenTickets;

public class AvailableGoldenTicketFinder
{
	private GoldenTicket availableGoldenTicket;

	private IEnumerable<GoldenTicket> source;

	public GoldenTicket AvailableGoldenTicket => availableGoldenTicket;

	public void Set(IEnumerable<GoldenTicket> source)
	{
		this.source = source;
	}

	public bool CheckActiveTicket()
	{
		if (source == null)
		{
			return false;
		}
		if (availableGoldenTicket != null && source.Contains(availableGoldenTicket) && availableGoldenTicket.LockWithTimer.IsOpen.Value)
		{
			return true;
		}
		IOrderedEnumerable<GoldenTicket> orderedEnumerable = source.OrderBy((GoldenTicket ticket) => ticket.ShowPriority);
		availableGoldenTicket = orderedEnumerable.FirstOrDefault((GoldenTicket ticket) => ticket.IsAvailableToShow);
		return availableGoldenTicket != null;
	}
}
