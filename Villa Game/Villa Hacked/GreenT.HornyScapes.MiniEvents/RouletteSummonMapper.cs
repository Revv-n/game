using System;
using GreenT.HornyScapes.Data;
using GreenT.HornyScapes.Lootboxes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using StripClub.Model;
using StripClub.Model.Shop.Data;

namespace GreenT.HornyScapes.MiniEvents;

[Serializable]
[Mapper]
public class RouletteSummonMapper
{
	public int id;

	public int garant_id;

	public int price;

	public int price_x10;

	public int reward;

	public int main_reward;

	public string source;

	public string price_resource;

	public string view_type;

	[JsonProperty("main_reward_type", ItemConverterType = typeof(StringEnumConverter))]
	public RewType[] main_reward_type;

	public string[] main_reward_id;

	[JsonProperty("secondary_reward_type", ItemConverterType = typeof(StringEnumConverter))]
	public RewType[] secondary_reward_type;

	public string[] secondary_reward_id;

	public UnlockType[] unlock_type;

	public string[] unlock_value;

	public ContentSource content_source;
}
