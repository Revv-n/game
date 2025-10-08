using GreenT.HornyScapes.Data;
using JetBrains.Annotations;
using StripClub.Model.Shop.Data;

namespace StripClub.Model.Shop;

[Mapper]
public class SubscriptionLotMapper : LotMapper
{
	public decimal price;

	public string price_resource;

	public decimal? prev_price;

	public string prev_price_resource;

	public int? extension_id;

	[CanBeNull]
	public int[] booster_lootbox_id;

	[CanBeNull]
	public int[] recharge_lootbox_id;

	public int[] lootbox_id;

	public ContentSource content_source;

	public string view_prefab;

	public string view_parameters;

	public string lot_id;

	public string bundle_name;

	public string bundle_title;

	public string bundle_descr;

	public string item_name;

	public string item_descr;

	public string image_name;

	public bool hot;

	public bool best;

	public bool sale;

	public int? sale_value;
}
