using System.Collections.Generic;
using GreenT.HornyScapes.Bank.BankTabs;
using GreenT.Types;

namespace GreenT.HornyScapes;

public sealed class BankTabClusterCombiner : ClusterCombiner<BankTab, BankTab.Manager>
{
	public BankTabClusterCombiner(IDictionary<ContentType, BankTab.Manager> cluster)
		: base(cluster)
	{
	}
}
