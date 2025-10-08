using StripClub.Model;

namespace GreenT.HornyScapes;

public abstract class InProgressLocker : Locker
{
	public readonly string Type;

	public InProgressLocker(string type)
	{
		Type = type;
	}

	public void Set(string type, bool newIsOpen)
	{
		if (Type == type)
		{
			isOpen.Value = newIsOpen;
		}
	}
}
