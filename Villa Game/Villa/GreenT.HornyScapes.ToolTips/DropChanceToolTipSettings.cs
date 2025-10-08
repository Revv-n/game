using System.Collections.Generic;

namespace GreenT.HornyScapes.ToolTips;

public class DropChanceToolTipSettings : TailedToolTipSettings
{
	public Dictionary<int, decimal> Chances = new Dictionary<int, decimal>();
}
