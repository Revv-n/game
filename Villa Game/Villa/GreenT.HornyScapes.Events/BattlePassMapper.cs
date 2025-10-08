using System;
using GreenT.HornyScapes.Data;
using GreenT.HornyScapes.Lootboxes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GreenT.HornyScapes.Events;

[Serializable]
[Mapper]
public class BattlePassMapper : IEventMapper
{
	public int bp_id;

	public int target_cost;

	[JsonProperty]
	public int[][] levels_cost_key;

	public int[] levels_cost_value;

	public int[] target_levels_free;

	public int[] target_levels_premium;

	[JsonProperty("rew_type_free", ItemConverterType = typeof(StringEnumConverter))]
	public RewType[] rew_type_free;

	public string[] rew_id_free;

	public int[] rew_qty_free;

	[JsonProperty("rew_type_premium", ItemConverterType = typeof(StringEnumConverter))]
	public RewType[] rew_type_premium;

	public string[] rew_id_premium;

	public int[] rew_qty_premium;

	public int[] is_light_free;

	public int[] is_light_premium;

	public string bp_bundle;

	public string[] any_lot_bought;

	public string go_to_banktab;

	public int[] merged_levels;

	public int[] block_levels_free;

	public long[] date_free;

	public int[] block_levels_premium;

	public long[] date_premium;

	public string[] bp_view;

	public string bp_resource;

	public int bp_preview_girl_id;

	public int ID => bp_id;

	public string Bundle => bp_bundle;
}
