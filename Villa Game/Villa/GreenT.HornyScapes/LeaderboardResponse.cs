using System;

namespace GreenT.HornyScapes;

[Serializable]
public sealed class LeaderboardResponse
{
	public Leaderboard global { get; set; }

	public Leaderboard group { get; set; }
}
