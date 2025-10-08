using System;
using GreenT.HornyScapes.Data;
using GreenT.HornyScapes.MiniEvents;

namespace GreenT.HornyScapes;

[Serializable]
[Mapper]
public class RouletteBankSummonMapper : RouletteSummonMapper
{
	public int bank_tab_id;

	public int go_to_banktab;
}
