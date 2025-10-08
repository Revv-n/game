using System;
using Newtonsoft.Json;

namespace GreenT.HornyScapes;

[Serializable]
public class PromocodeRewardResponse
{
	[JsonProperty]
	private PromocodeReward rewards;

	[JsonIgnore]
	public PromocodeReward Reward => rewards;
}
