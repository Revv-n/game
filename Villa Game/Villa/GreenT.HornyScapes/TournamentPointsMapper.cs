using System;
using GreenT.HornyScapes.Data;

namespace GreenT.HornyScapes;

[Serializable]
[Mapper]
public sealed class TournamentPointsMapper
{
	public int lower_border;

	public int upper_border;

	public float tournament_points;
}
