using GreenT.HornyScapes.Data;

namespace StripClub.Model.Shop.Data;

[Mapper]
public class ShopBundleMapper : LotMapper
{
	public decimal price;

	public string price_resource;

	public decimal? prev_price;

	public string prev_price_resource;

	public int lootbox_id;

	public ContentSource content_source;

	public string view_prefab;

	public string view_parameters;

	public bool hot;

	public bool best;

	public bool sale;

	public int? sale_value;

	public string lot_id;

	public string bundle_name;

	public string bundle_title;

	public string bundle_descr;

	public string item_name;

	public string item_descr;

	public string image_name;

	public int go_to_banktab;

	public bool resettable;

	public string[] reset_after;
}
