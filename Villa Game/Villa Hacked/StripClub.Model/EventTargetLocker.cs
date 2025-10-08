namespace StripClub.Model;

public class EventTargetLocker : EqualityLocker<int>
{
	public EventTargetLocker(int targetValue, Restriction restrictor)
		: base(targetValue, restrictor)
	{
	}
}
