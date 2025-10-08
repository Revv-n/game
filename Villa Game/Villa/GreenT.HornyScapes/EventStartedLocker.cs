using StripClub.Model;

namespace GreenT.HornyScapes;

public class EventStartedLocker : Locker
{
	public readonly string eventType;

	public EventStartedLocker(string eventType)
	{
		this.eventType = eventType;
	}

	public void Set(string eventType)
	{
		isOpen.Value = this.eventType == eventType;
	}
}
