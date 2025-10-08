namespace GreenT.HornyScapes.Analytics;

public class EventTaskAmplitudeEvent : AmplitudeEvent
{
	private const string ANALYTIC_EVENT = "event_task";

	private const string EVENT_ID = "event_id";

	public EventTaskAmplitudeEvent(int id, int eventId)
		: base("event_task")
	{
		AddEventParams("event_task", id);
		AddEventParams("event_id", eventId);
	}
}
