using System;
using GreenT.HornyScapes.Data;

namespace GreenT.HornyScapes.MiniEvents;

[Serializable]
[Mapper]
public sealed class ActivityShopMapper
{
	public int tab_id;

	public int bank_tab_id;

	public ShopTabType tab_type;

	public string icon;

	public string shop_bg;

	public string bundle;
}
