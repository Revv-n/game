using System;
using System.Collections.Generic;

namespace GreenT.HornyScapes;

[Serializable]
public sealed class Leaderboard
{
	public List<ScoreboardOpponentInfo> leaderboard;

	public ScoreboardPlayerInfo player;
}
