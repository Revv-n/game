using StripClub.Model;

namespace GreenT.HornyScapes.Tutorial;

public class TutorialLockerComponent : BaseTutorialComponent<TutorialLockerStepSO>
{
	public CompositeLocker Lockers;

	public TutorialLockerComponent(TutorialLockerStepSO stepSO, TutorialGroupManager groupManager)
		: base(stepSO, groupManager)
	{
		GetStep(stepSO.GroupID, stepSO.StepID);
		InnerInit(stepSO.GroupID, stepSO.StepID, stepSO.BlockScreen, stepSO.IsLight);
	}

	protected override void GetStep(int groupId, int stepId)
	{
		if (groupManager.GetStep(groupId, stepId) is TutorialLockerStep tutorialLockerStep)
		{
			base.StepModel = tutorialLockerStep;
			Lockers = tutorialLockerStep.Lockers;
		}
	}
}
