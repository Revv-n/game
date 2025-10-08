using StripClub.Model.Shop;
using UnityEngine;

namespace StripClub.UI.Shop;

public abstract class LotFeaturesView : MonoView<LotFeatures>
{
	[SerializeField]
	protected string hotStickerKey = "ui.shop.stickers.hot";

	[SerializeField]
	protected string bestStickerKey = "ui.shop.stickers.best";

	[SerializeField]
	protected string saleValueKey = "ui.shop.stickers.sale";

	[SerializeField]
	protected string extraValueKey = "ui.shop.stickers.extra";
}
