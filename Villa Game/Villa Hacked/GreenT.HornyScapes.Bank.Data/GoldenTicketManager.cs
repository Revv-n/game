using GreenT.Model.Collections;

namespace GreenT.HornyScapes.Bank.Data;

public class GoldenTicketManager : SimpleManager<GoldenTicket>
{
	public void Initialize()
	{
		foreach (GoldenTicket item in Collection)
		{
			item.Initialize();
		}
	}
}
