namespace GreenT.HornyScapes;

public class EnableFromLocker : IntervalLocker
{
	public EnableFromLocker(long from, long to)
		: base(from, to)
	{
	}

	public override void Set(long current)
	{
		isOpen.Value = current >= from && current <= to;
	}
}
