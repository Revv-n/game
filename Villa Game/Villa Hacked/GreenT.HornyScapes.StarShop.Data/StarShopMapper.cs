using System;
using GreenT.HornyScapes.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using StripClub.Model;

namespace GreenT.HornyScapes.StarShop.Data;

[Serializable]
[Mapper]
public class StarShopMapper
{
	public int step_id;

	public int reward;

	public int req_value;

	[JsonProperty("req_id", ItemConverterType = typeof(StringEnumConverter))]
	public CurrencyType req_id;

	public UnlockType[] unlock_type;

	public string[] unlock_value;

	public int object_number;

	public int room_id;
}
