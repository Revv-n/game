using System;
using GreenT.HornyScapes.Data;

namespace GreenT.HornyScapes.MiniEvents;

[Serializable]
[Mapper]
public class GarantChanceMapper
{
	public int garant_id;

	public int summon_qty;

	public float chance_value;
}
