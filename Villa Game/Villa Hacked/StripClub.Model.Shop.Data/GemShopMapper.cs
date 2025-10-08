using System;
using GreenT.HornyScapes.Data;
using GreenT.HornyScapes.Lootboxes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace StripClub.Model.Shop.Data;

[Serializable]
[Mapper]
public class GemShopMapper : LotMapper
{
	public bool first_purchase;

	public bool hot;

	public bool best;

	public bool sale;

	public int? sale_value;

	public string lot_id;

	public decimal price;

	public string price_resource;

	public decimal? prev_price;

	public string reward_id;

	public int reward_count;

	public int? prev_count;

	public int? extra_reward_count;

	public string extra_reward_id;

	public string local_key;

	public string icon;

	public ContentSource content_source;

	[JsonProperty("reward_type", ItemConverterType = typeof(StringEnumConverter))]
	public RewType reward_type;

	public string extra_reward_type;

	public string item_name;

	public string item_descr;

	public string image_name;
}
