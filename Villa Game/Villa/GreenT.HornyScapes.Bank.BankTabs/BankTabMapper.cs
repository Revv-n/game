using System;
using GreenT.HornyScapes.Data;
using GreenT.Types;
using Newtonsoft.Json;
using StripClub.Model;
using StripClub.Model.Shop.Data;
using UnityEngine;

namespace GreenT.HornyScapes.Bank.BankTabs;

[Serializable]
[Mapper]
public class BankTabMapper : ILimitedContent
{
	public int id;

	public int position;

	public LayoutType layout_type;

	public UnlockType[] unlock_type;

	public string[] unlock_value;

	public string parameters;

	public string icon;

	public ContentSource content_source;

	[JsonProperty(PropertyName = "type")]
	[field: SerializeField]
	public ConfigContentType Type { get; private set; }
}
