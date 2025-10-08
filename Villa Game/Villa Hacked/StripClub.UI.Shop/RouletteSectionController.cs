using GreenT.HornyScapes.Bank.BankTabs;

namespace StripClub.UI.Shop;

public sealed class RouletteSectionController : AbstractSectionController<LayoutType, BankTab, RouletteLotSectionView>
{
	protected override RouletteLotSectionView GetSection()
	{
		return sectionManager.GetView(base.Source.Layout);
	}
}
