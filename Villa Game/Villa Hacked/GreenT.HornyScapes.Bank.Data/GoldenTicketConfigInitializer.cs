using System.Collections.Generic;
using GreenT.HornyScapes.Data;
using GreenT.Types;
using Zenject;

namespace GreenT.HornyScapes.Bank.Data;

public class GoldenTicketConfigInitializer : LimitedContentConfigStructureInitializer<GoldenTicketMapper, GoldenTicket, GoldenTicketManager>
{
	public GoldenTicketConfigInitializer(IDictionary<ContentType, GoldenTicketManager> dictionary, GoldenTicketFactory factory, IEnumerable<IStructureInitializer> others = null)
		: base(dictionary, (IFactory<GoldenTicketMapper, GoldenTicket>)factory, others)
	{
	}
}
