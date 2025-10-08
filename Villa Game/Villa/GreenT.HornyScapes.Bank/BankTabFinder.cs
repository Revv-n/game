using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Bank.BankTabs;
using GreenT.Types;

namespace GreenT.HornyScapes.Bank;

public class BankTabFinder
{
	private readonly IDictionary<ContentType, BankTab.Manager> _managerCluster;

	private BankTab.Manager tabManager;

	public BankTabFinder(IDictionary<ContentType, BankTab.Manager> managerCluster)
	{
		_managerCluster = managerCluster;
	}

	public void Set(BankTab.Manager tabManager)
	{
		this.tabManager = tabManager;
	}

	public IEnumerable<BankTab> GetTabs()
	{
		return _managerCluster.Values.SelectMany((BankTab.Manager x) => x.Collection);
	}

	public bool TryGetActiveTab(LayoutType layoutType, out int id)
	{
		id = -1;
		BankTab bankTab = tabManager.Collection.FirstOrDefault((BankTab tab) => tab.Layout == layoutType && tab.Lock.IsOpen.Value);
		if (bankTab != null)
		{
			id = bankTab.ID;
		}
		return id != -1;
	}
}
