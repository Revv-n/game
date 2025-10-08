using StripClub.Model;
using UniRx;

namespace GreenT.HornyScapes.Lockers;

public class PermanentLocker : Locker
{
	public PermanentLocker(bool isOpen)
	{
		base.isOpen = new ReactiveProperty<bool>(isOpen);
	}

	public override void Initialize()
	{
	}
}
