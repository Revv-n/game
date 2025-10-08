using StripClub.Model;

namespace GreenT.HornyScapes.Lockers;

public class SimpleLocker : Locker
{
	private readonly bool isOpenByDefault;

	public SimpleLocker(bool isOpenByDefault = false)
	{
		isOpen.Value = isOpenByDefault;
		this.isOpenByDefault = isOpenByDefault;
	}

	public override void Initialize()
	{
		isOpen.Value = isOpenByDefault;
	}

	public void Set(bool isOpen)
	{
		base.isOpen.Value = isOpen;
	}
}
