using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.ToolTips;

namespace GreenT.HornyScapes.Tutorial;

public class TutorialClickCountStep : TutorialStep
{
	public string Id { get; }

	public int Count { get; }

	public TutorialClickCountStep(TutorialClickCountStepSO data, ToolTipTutorialOpener toolTipTutorialOpener, TutorialLightningSystem lightningSystem)
		: base(data, toolTipTutorialOpener, lightningSystem)
	{
		Id = data.Id;
		Count = data.Count;
	}
}
