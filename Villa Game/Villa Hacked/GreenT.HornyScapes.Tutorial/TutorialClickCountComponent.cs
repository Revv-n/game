namespace GreenT.HornyScapes.Tutorial;

public class TutorialClickCountComponent : BaseTutorialComponent<TutorialClickCountStepSO>
{
	public string Id => stepSO.Id;

	public int Count => stepSO.Count;

	public TutorialClickCountComponent(TutorialClickCountStepSO stepSO, TutorialGroupManager groupManager)
		: base(stepSO, groupManager)
	{
		GetStep(stepSO.GroupID, stepSO.StepID);
		InnerInit(stepSO.GroupID, stepSO.StepID, stepSO.BlockScreen, stepSO.IsLight);
	}

	protected override void GetStep(int groupId, int stepId)
	{
		base.StepModel = groupManager.GetStep(stepSO.GroupID, stepSO.StepID) as TutorialClickCountStep;
		_ = base.StepModel;
	}
}
