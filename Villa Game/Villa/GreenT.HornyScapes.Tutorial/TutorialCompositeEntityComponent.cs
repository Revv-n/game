using GreenT.Types;

namespace GreenT.HornyScapes.Tutorial;

public class TutorialCompositeEntityComponent : BaseTutorialComponent<TutorialCompositeEntityStepSO>
{
	public CompositeIdentificator CompositeIDs { get; }

	public TutorialCompositeEntityComponent(TutorialCompositeEntityStepSO stepSO, TutorialGroupManager groupManager)
		: base(stepSO, groupManager)
	{
		CompositeIDs = new CompositeIdentificator(stepSO.CompositeIDs);
		GetStep(stepSO.GroupID, stepSO.StepID);
		InnerInit(stepSO.GroupID, stepSO.StepID, stepSO.BlockScreen, stepSO.IsLight);
	}

	protected override void GetStep(int groupId, int stepId)
	{
		base.StepModel = groupManager.GetStep(stepSO.GroupID, stepSO.StepID) as TutorialCompositeEntityStep;
		_ = base.StepModel;
	}
}
