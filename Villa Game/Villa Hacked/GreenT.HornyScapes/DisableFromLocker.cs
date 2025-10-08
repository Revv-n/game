namespace GreenT.HornyScapes;

public class DisableFromLocker : IntervalLocker
{
	public DisableFromLocker(long from, long to)
		: base(from, to)
	{
	}

	public override void Set(long current)
	{
		isOpen.Value = from >= current || to <= current;
	}
}
