using UnityEngine;

namespace GreenT.HornyScapes.ToolTips;

public class ToolTipTutorialSettings : ToolTipSettings
{
	[HideInInspector]
	public bool OpenHint;

	[HideInInspector]
	public bool CloseHint;

	[HideInInspector]
	public int StepID;
}
