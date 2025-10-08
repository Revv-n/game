using System.Linq;
using GreenT.HornyScapes;
using StripClub.Model.Shop;

namespace StripClub.UI.Shop;

public class SubscriptionLotViewFactory : AbstractLotViewFactory<LotContainer, ContainerView>
{
	public override ContainerView Create(LotContainer lotContainer)
	{
		SubscriptionLot subscriptionLot = (SubscriptionLot)lotContainer.Lots.ToList()[0];
		SubscriptionLotView prefab = TryGetView<SubscriptionLotView>(subscriptionLot.Settings.ContentSource, subscriptionLot.Settings.PrefabKey);
		ContainerView containerView = container.InstantiatePrefabForComponent<ContainerView>(prefab, viewContainer);
		containerView.Set(new LotContainer(new Lot[1] { subscriptionLot }));
		return containerView;
	}
}
