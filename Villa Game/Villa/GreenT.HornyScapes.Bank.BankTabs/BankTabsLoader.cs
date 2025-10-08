using System.Collections.Generic;
using GreenT.Data;
using GreenT.Types;
using StripClub.Model.Data;
using Zenject;

namespace GreenT.HornyScapes.Bank.BankTabs;

public class BankTabsLoader : LimitedContentLoader<BankTabMapper, BankTab, BankTab.Manager>
{
	public BankTabsLoader(ILoader<IEnumerable<BankTabMapper>> loader, IFactory<BankTabMapper, BankTab> factory, IDictionary<ContentType, BankTab.Manager> cluster)
		: base(loader, factory, cluster)
	{
	}
}
