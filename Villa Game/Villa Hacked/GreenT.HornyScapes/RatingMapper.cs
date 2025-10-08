using System;
using GreenT.HornyScapes.Data;

namespace GreenT.HornyScapes;

[Serializable]
[Mapper]
public sealed class RatingMapper
{
	public int id;

	public int range;

	public int matchmaking;

	public string currency;
}
