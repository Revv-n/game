using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.ToolTips;

namespace GreenT.HornyScapes.Tutorial;

public class TutorialEntityStep : TutorialStep
{
	public int UniqID { get; }

	public TutorialEntityStep(TutorialEntityStepSO data, ToolTipTutorialOpener toolTipTutorialOpener, TutorialLightningSystem lightningSystem)
		: base(data, toolTipTutorialOpener, lightningSystem)
	{
		UniqID = data.UniqID;
	}
}
