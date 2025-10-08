using GreenT.HornyScapes.Bank.Data;
using GreenT.HornyScapes.Subscription.Push;
using StripClub.UI.Shop;

namespace GreenT.HornyScapes.Bank.Offer.UI;

public class SubscriptionOfferSectionController : AbstractSectionController<LayoutType, SubscriptionPushSettings, SubscriptionOfferSectionView>
{
	protected override SubscriptionOfferSectionView GetSection()
	{
		return sectionManager.GetView(base.Source.Layout);
	}
}
