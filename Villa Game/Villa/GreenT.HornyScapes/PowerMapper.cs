using System;
using GreenT.HornyScapes.Data;

namespace GreenT.HornyScapes;

[Serializable]
[Mapper]
public sealed class PowerMapper
{
	public int id;

	public int rare_promote_coef;

	public int epic_promote_coef;

	public int legendary_promote_coef;

	public int mythic_promote_coef;
}
