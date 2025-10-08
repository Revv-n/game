using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.ToolTips;
using GreenT.Types;

namespace GreenT.HornyScapes.Tutorial;

public class TutorialCompositeEntityStep : TutorialStep
{
	public CompositeIdentificator UniqID { get; }

	public TutorialCompositeEntityStep(TutorialCompositeEntityStepSO data, ToolTipTutorialOpener toolTipTutorialOpener, TutorialLightningSystem lightningSystem)
		: base(data, toolTipTutorialOpener, lightningSystem)
	{
		UniqID = new CompositeIdentificator(data.CompositeIDs);
	}
}
