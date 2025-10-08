using System.Linq;
using GreenT.HornyScapes;
using StripClub.Model.Shop;
using UnityEngine;

namespace StripClub.UI.Shop;

public class SubscriptionLotViewFactory : AbstractLotViewFactory<LotContainer, ContainerView>
{
	public override ContainerView Create(LotContainer lotContainer)
	{
		SubscriptionLot subscriptionLot = (SubscriptionLot)lotContainer.Lots.ToList()[0];
		SubscriptionLotView subscriptionLotView = TryGetView<SubscriptionLotView>(subscriptionLot.Settings.ContentSource, subscriptionLot.Settings.PrefabKey);
		ContainerView containerView = container.InstantiatePrefabForComponent<ContainerView>((Object)subscriptionLotView, viewContainer);
		containerView.Set(new LotContainer(new Lot[1] { subscriptionLot }));
		return containerView;
	}
}
