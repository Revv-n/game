using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.ToolTips;
using StripClub.Model;

namespace GreenT.HornyScapes.Tutorial;

public class TutorialLockerStep : TutorialStep
{
	public CompositeLocker Lockers;

	public TutorialLockerStep(TutorialLockerStepSO data, ILocker[] lockers, ToolTipTutorialOpener toolTipTutorialOpener, TutorialLightningSystem lightningSystem)
		: base(data, toolTipTutorialOpener, lightningSystem)
	{
		Lockers = new CompositeLocker(lockers);
	}
}
