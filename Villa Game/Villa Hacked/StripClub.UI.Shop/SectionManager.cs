using System.Linq;
using GreenT.HornyScapes.Bank.BankTabs;

namespace StripClub.UI.Shop;

public class SectionManager : AbstractSectionManager<LayoutType, BankTab, BankSectionView>
{
	protected override BankSectionView SelectViewByKey(LayoutType key)
	{
		return views.FirstOrDefault((BankSectionView _view) => _view.Source.Layout.Equals(key));
	}
}
