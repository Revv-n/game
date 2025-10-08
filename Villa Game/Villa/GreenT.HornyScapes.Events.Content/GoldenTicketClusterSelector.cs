using GreenT.HornyScapes.Bank.GoldenTickets;
using GreenT.Types;

namespace GreenT.HornyScapes.Events.Content;

public class GoldenTicketClusterSelector : IContentSelector, ISelector<ContentType>
{
	private ContentType currentType;

	private readonly GoldenTicketManagerCluster managerCluster;

	private readonly AvailableGoldenTicketFinder availableGoldenTicketFinder;

	public GoldenTicketClusterSelector(GoldenTicketManagerCluster managerCluster, AvailableGoldenTicketFinder availableGoldenTicketFinder)
	{
		this.managerCluster = managerCluster;
		this.availableGoldenTicketFinder = availableGoldenTicketFinder;
	}

	public void Select(ContentType selector)
	{
		currentType = selector;
		availableGoldenTicketFinder.Set(managerCluster[currentType].Collection);
	}
}
