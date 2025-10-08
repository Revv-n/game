using System;
using GreenT.HornyScapes.Lootboxes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GreenT.HornyScapes.Relationships.Mappers;

[Serializable]
public class RelationshipRewardMapper
{
	public int id;

	public int promote_to_unlock;

	public int points_to_unlock;

	[JsonProperty("rewards_type", ItemConverterType = typeof(StringEnumConverter))]
	public RewType[] rewards_type;

	public string[] rewards_id;

	public int[] rewards_qty;

	public int status_number;

	public string date_background;
}
