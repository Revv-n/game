using System;
using GreenT.HornyScapes.Data;
using GreenT.HornyScapes.Lootboxes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GreenT.HornyScapes.Meta.Duplicates;

[Serializable]
[Mapper]
public class DuplicateRewardMapper
{
	public int id;

	[JsonProperty("rew_type", ItemConverterType = typeof(StringEnumConverter))]
	public RewType[] rew_type;

	public string[] rew_id;

	public int[] rew_qty;
}
