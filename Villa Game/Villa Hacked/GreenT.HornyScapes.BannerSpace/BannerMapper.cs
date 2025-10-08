using System;
using GreenT.HornyScapes.Data;
using GreenT.HornyScapes.Lootboxes;
using GreenT.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using StripClub.Model;
using UnityEngine;

namespace GreenT.HornyScapes.BannerSpace;

[Serializable]
[Mapper]
public class BannerMapper : ILimitedContent
{
	public int id;

	public string source;

	public int bank_tab_id;

	public string banner_group;

	public string content_source;

	public string background_name;

	public string buy_resource;

	public int price_x1;

	public int price_x10;

	public string rebuy_resource;

	public int rebuy_cost_1x;

	public int main_reward_id;

	public int legendary_reward_id;

	public int epic_reward_id;

	public int rare_reward_id;

	public int[] main_reward_chances;

	public int[] epic_reward_chances;

	public int garant_id;

	[JsonProperty("details_legendary_rewards_type", ItemConverterType = typeof(StringEnumConverter))]
	public RewType[] details_legendary_rewards_type;

	public string[] details_legendary_rewards_value_qty;

	public int[] details_legendary_rewards_chances;

	public int[] details_legendary_new;

	public int[] details_legendary_main;

	[JsonProperty("details_epic_rewards_type", ItemConverterType = typeof(StringEnumConverter))]
	public RewType[] details_epic_rewards_type;

	public string[] details_epic_rewards_value_qty;

	public int[] details_epic_rewards_chances;

	[JsonProperty("details_rare_rewards_type", ItemConverterType = typeof(StringEnumConverter))]
	public RewType[] details_rare_rewards_type;

	public string[] details_rare_rewards_value_qty;

	public int[] details_rare_rewards_chances;

	public UnlockType[] unlock_type;

	public string[] unlock_value;

	[JsonProperty(PropertyName = "type")]
	[field: SerializeField]
	public ConfigContentType Type { get; private set; }
}
