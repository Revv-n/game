using System;
using GreenT.HornyScapes.Data;
using GreenT.Types;
using Newtonsoft.Json;
using StripClub.Model;
using UnityEngine;

namespace GreenT.HornyScapes.MergeStore;

[Serializable]
[Mapper]
public class MergeStoreMapper : ILimitedContent
{
	public int id;

	public string target_id;

	public UnlockType[] unlock_type;

	public string[] unlock_value;

	public int regular_section_refresh_time;

	public int regular_section_refresh_cost;

	public CurrencyType regular_section_refresh_currency;

	public int[] regular_section_discount_chances;

	public int energy_threshold;

	public int low_energy_lower_tier_chance;

	public int premium_section_refresh_time;

	public int premium_section_refresh_cost;

	public CurrencyType premium_section_refresh_currency;

	public int[] premium_section_discount_chances;

	public int sale_tier_difference;

	public bool use_unique_premium;

	public int[] discount_rarity_chance;

	[JsonProperty(PropertyName = "type")]
	[field: SerializeField]
	public ConfigContentType Type { get; private set; }
}
