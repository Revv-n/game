using StripClub.Model;

namespace GreenT.HornyScapes.Tutorial;

public class TutorialGroupLocker : Locker
{
	public int GroupID { get; }

	public TutorialGroupLocker(int tutor_id)
	{
		GroupID = tutor_id;
	}

	public void Set(TutorialGroupSteps item)
	{
		if (item.GroupID == GroupID)
		{
			isOpen.Value = item.IsCompleted.Value;
		}
	}
}
