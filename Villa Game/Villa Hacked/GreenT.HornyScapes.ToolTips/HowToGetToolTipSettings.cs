using System.Collections.Generic;
using Merge;
using UnityEngine;

namespace GreenT.HornyScapes.ToolTips;

public class HowToGetToolTipSettings : TailedToolTipSettings
{
	[Header("Подставляется в рантайме")]
	public List<GIConfig> HowToGet;

	public List<Sprite> AdditionalWays = new List<Sprite>();
}
