using System;
using Newtonsoft.Json;

namespace GreenT.HornyScapes;

[Serializable]
public class PromocodeReward
{
	[JsonProperty]
	private int SOFT_CURRENCY;

	[JsonProperty]
	private int HARD_CURRENCY;

	[JsonIgnore]
	public int SoftCurrency => SOFT_CURRENCY;

	[JsonIgnore]
	public int HardCurrency => HARD_CURRENCY;
}
