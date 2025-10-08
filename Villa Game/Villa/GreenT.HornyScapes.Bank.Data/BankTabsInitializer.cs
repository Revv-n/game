using System.Collections.Generic;
using GreenT.HornyScapes.Bank.BankTabs;
using GreenT.HornyScapes.Data;
using GreenT.Types;
using Zenject;

namespace GreenT.HornyScapes.Bank.Data;

public class BankTabsInitializer : LimitedContentConfigStructureInitializer<BankTabMapper, BankTab, BankTab.Manager>
{
	public BankTabsInitializer(IDictionary<ContentType, BankTab.Manager> dictionary, IFactory<BankTabMapper, BankTab> factory, IEnumerable<IStructureInitializer> others = null)
		: base(dictionary, factory, others)
	{
	}
}
