using GreenT.HornyScapes.Tasks;
using StripClub.Model;
using StripClub.Model.Quest;

namespace GreenT.HornyScapes;

public class TaskLocker : Locker
{
	public int ItemID { get; }

	public StateType State { get; }

	public TaskLocker(int itemID, StateType state)
	{
		ItemID = itemID;
		State = state;
	}

	public void Set(Task item)
	{
		if (item.ID == ItemID)
		{
			isOpen.Value = (State & item.State) == item.State;
		}
	}
}
