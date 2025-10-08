namespace GreenT.HornyScapes;

public class EventInProgressLocker : InProgressLocker
{
	public EventInProgressLocker(string type)
		: base(type)
	{
	}
}
