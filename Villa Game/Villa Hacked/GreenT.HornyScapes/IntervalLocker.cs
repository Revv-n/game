using StripClub.Model;

namespace GreenT.HornyScapes;

public abstract class IntervalLocker : Locker
{
	protected readonly long from;

	protected readonly long to;

	public long To => to;

	public IntervalLocker(long from, long to)
	{
		this.from = from;
		this.to = to;
	}

	public abstract void Set(long current);
}
