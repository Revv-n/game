namespace GreenT.HornyScapes;

public class DisableFromEventLocker : EventIntervalLocker
{
	public DisableFromEventLocker(long from, long to)
		: base(from, to)
	{
	}

	protected override void SetIsOpen(long time)
	{
		isOpen.Value = time <= from || time >= to;
	}
}
