namespace GreenT.HornyScapes.Tutorial;

public class TutorialEntityComponent : BaseTutorialComponent<TutorialEntityStepSO>
{
	public int UniqID => stepSO.UniqID;

	public TutorialEntityComponent(TutorialEntityStepSO stepSO, TutorialGroupManager groupManager)
		: base(stepSO, groupManager)
	{
		GetStep(stepSO.GroupID, stepSO.StepID);
		InnerInit(stepSO.GroupID, stepSO.StepID, stepSO.BlockScreen, stepSO.IsLight);
	}

	protected override void GetStep(int groupId, int stepId)
	{
		base.StepModel = groupManager.GetStep(stepSO.GroupID, stepSO.StepID) as TutorialEntityStep;
		_ = base.StepModel;
	}
}
