using System;
using GreenT.HornyScapes.Data;

namespace GreenT.HornyScapes;

[Serializable]
[Mapper]
public sealed class MatchmakingMapper
{
	public int id;

	public float player_power_from;

	public float player_power_to;

	public string[] range_value;

	public string[] range_rewards;
}
