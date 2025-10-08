using System.Collections.Generic;
using GreenT.HornyScapes.Bank.BankTabs;
using GreenT.HornyScapes.Bank.UI;
using GreenT.HornyScapes.Events.Content;
using GreenT.Types;
using GreenT.UI;

namespace GreenT.HornyScapes.Bank.Event;

public class BankContentSelector : IContentSelector, ISelector<ContentType>
{
	private readonly IWindowsManager windowsManager;

	private readonly IDictionary<ContentType, BankTab.Manager> managerCluster;

	private readonly BankTabUIController tabController;

	private readonly BankTabFinder bankTabFinder;

	private BankWindow bankWindow;

	private ContentType actualType;

	public ContentType GetActualType()
	{
		return actualType;
	}

	public BankContentSelector(IWindowsManager windowsManager, IDictionary<ContentType, BankTab.Manager> managerCluster, BankTabUIController tabController, BankTabFinder bankTabFinder)
	{
		this.windowsManager = windowsManager;
		this.managerCluster = managerCluster;
		this.tabController = tabController;
		this.bankTabFinder = bankTabFinder;
	}

	public void Select(ContentType type)
	{
		actualType = type;
		BankTab.Manager manager = managerCluster[type];
		tabController.Set(manager);
		bankTabFinder.Set(manager);
	}
}
