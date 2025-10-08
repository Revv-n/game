namespace GreenT.HornyScapes.Analytics.Bank;

public class FreeSummonAmplitudeEvent : BaseSummonAmplitudeEvent
{
	private const string EVENT_NAME_KEY = "free_summon";

	public FreeSummonAmplitudeEvent()
		: base("free_summon")
	{
	}
}
