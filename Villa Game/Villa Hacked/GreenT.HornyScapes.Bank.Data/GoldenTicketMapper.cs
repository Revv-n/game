using System;
using GreenT.HornyScapes.Data;
using GreenT.Types;
using Newtonsoft.Json;
using StripClub.Model;
using UnityEngine;

namespace GreenT.HornyScapes.Bank.Data;

[Serializable]
[Mapper]
public class GoldenTicketMapper : ILimitedContent
{
	public int id;

	public int bundle_id;

	public int priority;

	public float time;

	public float repeat_time;

	public float repeat_delta;

	public UnlockType[] unlock_type;

	public string[] unlock_value;

	[JsonProperty(PropertyName = "type")]
	[field: SerializeField]
	public ConfigContentType Type { get; private set; }
}
