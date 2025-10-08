using GreenT.Bonus;
using GreenT.HornyScapes.Data;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GreenT.HornyScapes.Booster;

[Mapper]
public class BoosterMapper
{
	public int booster_id;

	public long booster_time;

	[CanBeNull]
	public int[] tab_id;

	[CanBeNull]
	public string summon_type;

	[JsonProperty("bonus_type", ItemConverterType = typeof(StringEnumConverter))]
	public BonusType bonus_type;

	public int bonus_value;
}
