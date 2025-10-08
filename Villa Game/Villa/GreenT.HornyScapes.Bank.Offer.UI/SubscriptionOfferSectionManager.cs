using System.Linq;
using GreenT.HornyScapes.Bank.Data;
using GreenT.HornyScapes.Subscription.Push;
using StripClub.UI.Shop;

namespace GreenT.HornyScapes.Bank.Offer.UI;

public class SubscriptionOfferSectionManager : AbstractSectionManager<LayoutType, SubscriptionPushSettings, SubscriptionOfferSectionView>
{
	protected override SubscriptionOfferSectionView SelectViewByKey(LayoutType key)
	{
		return views.FirstOrDefault((SubscriptionOfferSectionView view) => view.Source.Layout.Equals(key));
	}
}
