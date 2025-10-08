using System.Linq;
using GreenT.HornyScapes.Bank.Data;
using StripClub.UI.Shop;

namespace GreenT.HornyScapes.Bank.Offer.UI;

public class SectionManager : AbstractSectionManager<LayoutType, OfferSettings, OfferSectionView>
{
	protected override OfferSectionView SelectViewByKey(LayoutType key)
	{
		return views.FirstOrDefault((OfferSectionView _view) => _view.Source.Layout.Equals(key));
	}
}
