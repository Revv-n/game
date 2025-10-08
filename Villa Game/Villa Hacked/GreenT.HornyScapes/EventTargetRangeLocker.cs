using StripClub.Model;

namespace GreenT.HornyScapes;

public class EventTargetRangeLocker : Locker
{
	private readonly int from;

	private readonly int to;

	public EventTargetRangeLocker(int from, int to)
	{
		this.from = from;
		this.to = to;
	}

	public void Set(int current)
	{
		isOpen.Value = current >= from && current <= to;
	}
}
