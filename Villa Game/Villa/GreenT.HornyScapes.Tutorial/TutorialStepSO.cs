using GreenT.HornyScapes.ToolTips;
using UnityEngine;

namespace GreenT.HornyScapes.Tutorial;

[CreateAssetMenu(fileName = "TutorialStep", menuName = "GreenT/Tutorial/Step/Step")]
public class TutorialStepSO : ScriptableObject
{
	public int StepID;

	public int GroupID;

	[Header("Don't repeat if complete")]
	public bool Skip;

	[Header("Hint settings:")]
	public bool ShowHint = true;

	public ToolTipTutorialSettings HintSettings;

	public bool OpenHint = true;

	public bool CloseHint = true;

	public float DelayBeforeStart;

	[Header("Arrow settings:")]
	public bool ShowArrow = true;

	public ToolTipArrowTutorialSettings ArrowSettings;

	public bool IsLight;

	[Header("Screen settings:")]
	public bool BlockScreen = true;

	public bool UseBlur;

	public bool BlockMerge = true;

	public bool ShowSettings;

	public bool UseMask;

	public MaskSettings[] MaskSettings;
}
