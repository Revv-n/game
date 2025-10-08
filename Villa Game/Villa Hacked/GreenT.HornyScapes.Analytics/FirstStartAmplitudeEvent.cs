namespace GreenT.HornyScapes.Analytics;

public class FirstStartAmplitudeEvent : AmplitudeEvent
{
	private const string EVENT_NAME_KEY = "first_start_game";

	public FirstStartAmplitudeEvent()
		: base("first_start_game")
	{
	}
}
