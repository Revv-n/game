using StripClub.Model;

namespace GreenT.HornyScapes;

public class StarMaxLocker : Locker
{
	public int StarCount { get; }

	public StarMaxLocker(int count)
	{
		StarCount = count;
	}

	public void Set(int count)
	{
		if (!isOpen.Value)
		{
			isOpen.Value = StarCount == count;
		}
	}
}
