using System.Linq;
using GreenT.HornyScapes;
using StripClub.Model.Shop;
using UnityEngine;

namespace StripClub.UI.Shop;

public class SummonLotViewFactory : AbstractLotViewFactory<LotContainer, ContainerView>
{
	public override ContainerView Create(LotContainer lotLotContainer)
	{
		SummonLot summonLot = (SummonLot)lotLotContainer.Lots.ToList()[0];
		LotView lotView = TryGetView<SummonLotView>(summonLot.Source, summonLot.ViewName);
		ContainerView containerView = container.InstantiatePrefabForComponent<ContainerView>((Object)lotView, viewContainer);
		containerView.Set(new LotContainer(new Lot[1] { summonLot }));
		return containerView;
	}
}
