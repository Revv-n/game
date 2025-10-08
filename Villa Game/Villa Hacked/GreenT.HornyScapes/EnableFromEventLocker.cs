namespace GreenT.HornyScapes;

public class EnableFromEventLocker : EventIntervalLocker
{
	public EnableFromEventLocker(long from, long to)
		: base(from, to)
	{
	}

	protected override void SetIsOpen(long time)
	{
		isOpen.Value = time >= from && time <= to;
	}
}
