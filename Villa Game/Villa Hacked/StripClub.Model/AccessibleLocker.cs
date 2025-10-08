namespace StripClub.Model;

public class AccessibleLocker : Locker
{
	public override void Initialize()
	{
		isOpen.Value = true;
	}
}
