using System;
using GreenT.HornyScapes.Data;
using GreenT.HornyScapes.Lootboxes;
using GreenT.HornyScapes.MiniEvents;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GreenT.HornyScapes.Events;

[Serializable]
[Mapper]
public class MiniEventMapper : IEventMapper
{
	public int id;

	public int activity_id;

	public int position;

	public string minitab_view;

	public string background;

	public string girl_view;

	public int[] currency_id;

	public string bundle;

	public string promo_view;

	[JsonProperty("config_type", ItemConverterType = typeof(StringEnumConverter))]
	public GreenT.HornyScapes.MiniEvents.ConfigType config_type;

	[JsonProperty("promo_type", ItemConverterType = typeof(StringEnumConverter))]
	public RewType[] promo_type;

	public string[] promo_id;

	public int[] promo_qty;

	public int ID => id;

	public string Bundle => bundle;
}
