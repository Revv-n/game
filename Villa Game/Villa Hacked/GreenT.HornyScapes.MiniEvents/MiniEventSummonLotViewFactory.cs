using StripClub.Model.Shop;
using UnityEngine;

namespace GreenT.HornyScapes.MiniEvents;

public class MiniEventSummonLotViewFactory : AbstractLotViewFactory<Lot, MiniEventShopSummonView>
{
	public override MiniEventShopSummonView Create(Lot lot)
	{
		SummonLot summonLot = (SummonLot)lot;
		MiniEventShopSummonView miniEventShopSummonView = TryGetView<MiniEventShopSummonView>(summonLot.Source, summonLot.ViewName);
		MiniEventShopSummonView miniEventShopSummonView2 = container.InstantiatePrefabForComponent<MiniEventShopSummonView>((Object)miniEventShopSummonView, viewContainer);
		miniEventShopSummonView2.Set(summonLot);
		return miniEventShopSummonView2;
	}
}
