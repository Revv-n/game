using System;
using GreenT.HornyScapes.Data;
using GreenT.HornyScapes.Lootboxes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GreenT.HornyScapes.Events;

[Serializable]
[Mapper]
public class EventMapper : IEventMapper
{
	public int event_id;

	public int[] event_targets;

	[JsonProperty("rew_type", ItemConverterType = typeof(StringEnumConverter))]
	public RewType[] rew_type;

	public string[] rew_id;

	public int[] rew_qty;

	public RewType[] preview_rew_type;

	public string[] preview_rew_id;

	public int[] preview_rew_qty;

	public int[] characters = new int[4] { 10001, 10002, 10003, 10004 };

	public string event_bundle;

	public int focus;

	public bool has_recipe_book;

	public bool is_separate_energy;

	public int bp_id;

	public int rating_id;

	public int group_rating_id;

	public string[] event_view;

	public int ID => event_id;

	public string Bundle => event_bundle;
}
