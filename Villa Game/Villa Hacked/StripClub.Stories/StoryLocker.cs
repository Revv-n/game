using GreenT.HornyScapes.Stories;
using StripClub.Model;

namespace StripClub.Stories;

public class StoryLocker : Locker
{
	public int ItemID { get; }

	public StoryLocker(int story_id)
	{
		ItemID = story_id;
	}

	public void Set(GreenT.HornyScapes.Stories.Story item)
	{
		if (item.ID == ItemID)
		{
			isOpen.Value = item.IsComplete;
		}
	}
}
