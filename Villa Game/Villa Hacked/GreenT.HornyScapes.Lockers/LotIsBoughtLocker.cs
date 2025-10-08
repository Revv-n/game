using StripClub.Model;
using StripClub.Model.Shop;

namespace GreenT.HornyScapes.Lockers;

public class LotIsBoughtLocker : Locker
{
	protected bool ValueOnEvent { get; private set; }

	public LotIsBoughtLocker(bool openOnEvent = true)
	{
		isOpen.Value = !openOnEvent;
		ValueOnEvent = openOnEvent;
	}

	public override void Initialize()
	{
		isOpen.Value = !ValueOnEvent;
	}

	public void Set(Lot lot)
	{
		if (lot.Received > 0 && isOpen.Value != ValueOnEvent)
		{
			isOpen.Value = ValueOnEvent;
		}
	}
}
