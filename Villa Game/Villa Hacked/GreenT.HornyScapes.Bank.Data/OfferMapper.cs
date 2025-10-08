using System;
using GreenT.HornyScapes.Data;
using GreenT.Types;
using Newtonsoft.Json;
using StripClub.Model;
using StripClub.Model.Shop.Data;
using UnityEngine;

namespace GreenT.HornyScapes.Bank.Data;

[Serializable]
[Mapper]
public class OfferMapper : ILimitedContent
{
	public int id;

	public int position;

	public int[] bundles;

	public LayoutType layout_type;

	public UnlockType[] unlock_type;

	public string[] unlock_value;

	public string view_parameters;

	public float time;

	public float repeat_time;

	public float repeat_delta;

	public float push_time;

	public bool end_on_lock;

	public ContentSource content_source;

	[JsonProperty(PropertyName = "type")]
	[field: SerializeField]
	public ConfigContentType Type { get; private set; }
}
