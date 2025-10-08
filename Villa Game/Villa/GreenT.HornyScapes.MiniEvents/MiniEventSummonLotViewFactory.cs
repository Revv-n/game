using StripClub.Model.Shop;

namespace GreenT.HornyScapes.MiniEvents;

public class MiniEventSummonLotViewFactory : AbstractLotViewFactory<Lot, MiniEventShopSummonView>
{
	public override MiniEventShopSummonView Create(Lot lot)
	{
		SummonLot summonLot = (SummonLot)lot;
		MiniEventShopSummonView prefab = TryGetView<MiniEventShopSummonView>(summonLot.Source, summonLot.ViewName);
		MiniEventShopSummonView miniEventShopSummonView = container.InstantiatePrefabForComponent<MiniEventShopSummonView>(prefab, viewContainer);
		miniEventShopSummonView.Set(summonLot);
		return miniEventShopSummonView;
	}
}
