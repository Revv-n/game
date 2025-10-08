using System.Linq;
using GreenT.HornyScapes.Bank.BankTabs;

namespace StripClub.UI.Shop;

public sealed class RouletteSectionManager : AbstractSectionManager<LayoutType, BankTab, RouletteLotSectionView>
{
	protected override RouletteLotSectionView SelectViewByKey(LayoutType key)
	{
		return views.FirstOrDefault((RouletteLotSectionView _view) => _view.Source.Layout.Equals(key));
	}
}
