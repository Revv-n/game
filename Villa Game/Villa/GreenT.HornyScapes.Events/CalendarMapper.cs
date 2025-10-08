using System;
using GreenT.HornyScapes.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using StripClub.Model;

namespace GreenT.HornyScapes.Events;

[Serializable]
[Mapper]
public class CalendarMapper
{
	public int id;

	public int event_id;

	[JsonProperty("event_type", ItemConverterType = typeof(StringEnumConverter))]
	public EventStructureType event_type;

	public int duration;

	public UnlockType[] unlock_type;

	public string[] unlock_value;

	public long last_chance_duration;
}
