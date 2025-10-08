namespace GreenT.HornyScapes.Tutorial;

public class TutorialComponent : BaseTutorialComponent<TutorialStepSO>
{
	public TutorialComponent(TutorialStepSO stepSO, TutorialGroupManager groupManager)
		: base(stepSO, groupManager)
	{
		GetStep(stepSO.GroupID, stepSO.StepID);
		InnerInit(stepSO.GroupID, stepSO.StepID, stepSO.BlockScreen, stepSO.IsLight);
	}

	protected override void GetStep(int groupId, int stepId)
	{
		base.StepModel = groupManager.GetStep(groupId, stepId);
		_ = base.StepModel;
	}
}
