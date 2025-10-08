using StripClub.Model.Shop;
using UnityEngine;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class MiniEventSingleSummonLotViewFactory : MiniEventSummonLotViewFactory
{
	[SerializeField]
	private Vector3 _position;

	public override MiniEventShopSummonView Create(Lot lot)
	{
		MiniEventShopSummonView miniEventShopSummonView = base.Create(lot);
		RectTransform component = miniEventShopSummonView.GetComponent<RectTransform>();
		component.anchorMin = new Vector2(0.5f, 0.5f);
		component.anchorMax = new Vector2(0.5f, 0.5f);
		component.pivot = new Vector2(0.5f, 0.5f);
		miniEventShopSummonView.transform.localPosition = _position;
		return miniEventShopSummonView;
	}
}
