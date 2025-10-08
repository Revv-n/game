namespace GreenT.HornyScapes.Analytics;

public class StartAmplitudeEvent : AmplitudeEvent
{
	private const string EVENT_NAME_KEY = "start_game";

	public StartAmplitudeEvent()
		: base("start_game")
	{
	}
}
