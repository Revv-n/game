using System;
using GreenT.HornyScapes.Lootboxes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using StripClub.Model;

namespace GreenT.HornyScapes.Tasks.Data;

[Serializable]
public class TaskMapper
{
	public int task_id;

	[JsonProperty("rew_type", ItemConverterType = typeof(StringEnumConverter))]
	public RewType[] rew_type;

	public string[] rew_id;

	public int[] rew_qty;

	public string[] req_items;

	public int[] req_value;

	public UnlockType[] unlock_type;

	public string[] unlock_value;

	[JsonProperty("content_type", ItemConverterType = typeof(StringEnumConverter))]
	public TaskContentType content_type;
}
