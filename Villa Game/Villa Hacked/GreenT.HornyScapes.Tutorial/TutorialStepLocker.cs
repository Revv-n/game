using StripClub.Model;

namespace GreenT.HornyScapes.Tutorial;

public class TutorialStepLocker : Locker
{
	public int ItemID { get; }

	public TutorialStepLocker(int tutor_id)
	{
		ItemID = tutor_id;
	}

	public void Set(TutorialStep item)
	{
		if (item.StepID == ItemID)
		{
			isOpen.Value = item.IsComplete.Value;
		}
	}
}
