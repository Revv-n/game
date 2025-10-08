using System.Collections.Generic;
using GreenT.HornyScapes.Bank.Data;
using GreenT.Types;

namespace GreenT.HornyScapes.Events.Content;

public class GoldenTicketManagerCluster : ContentCluster<GoldenTicketManager>
{
	public void Initialize()
	{
		using Enumerator enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			enumerator.Current.Value.Initialize();
		}
	}
}
