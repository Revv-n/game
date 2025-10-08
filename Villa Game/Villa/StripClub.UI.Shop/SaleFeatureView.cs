using StripClub.Model.Shop;
using UnityEngine;

namespace StripClub.UI.Shop;

public class SaleFeatureView : LotFeaturesView
{
	[SerializeField]
	private GameObject saleSticker;

	[SerializeField]
	private LocalizedTextMeshPro saleValue;

	public override void Set(LotFeatures features)
	{
		base.Set(features);
		if (features.Stickers.Contains(Stickers.Sale) && features.SaleValue.HasValue && features.SaleValue.Value != 0)
		{
			int value = features.SaleValue.Value;
			saleValue.Init((value > 0) ? extraValueKey : saleValueKey, features.SaleValue);
			saleSticker.SetActive(value: true);
		}
		else
		{
			saleSticker.SetActive(value: false);
		}
	}
}
