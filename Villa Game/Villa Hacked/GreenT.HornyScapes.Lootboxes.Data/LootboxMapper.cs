using System;
using GreenT.HornyScapes.Data;
using GreenT.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using StripClub.Model.Cards;

namespace GreenT.HornyScapes.Lootboxes.Data;

[Serializable]
[Mapper]
public class LootboxMapper
{
	public string chest_name;

	public int chest_id;

	public ContentType type;

	public Rarity rarity;

	public int? drop_count;

	[JsonProperty("rew_type", ItemConverterType = typeof(StringEnumConverter))]
	public RewType[] rew_type;

	public string[] rew_id;

	public int[] rew_qty;

	public int[] rew_delta;

	public int[] rew_chance;

	[JsonProperty("guaranteed_type", ItemConverterType = typeof(StringEnumConverter))]
	public RewType[] guaranteed_type;

	public string[] guaranteed_id;

	public int[] guaranteed_qty;

	public int[] guaranteed_delta;
}
