using GreenT.HornyScapes.Bank.BankTabs;

namespace StripClub.UI.Shop;

public class SectionController : AbstractSectionController<LayoutType, BankTab, BankSectionView>
{
	protected override BankSectionView GetSection()
	{
		return sectionManager.GetView(base.Source.Layout);
	}
}
