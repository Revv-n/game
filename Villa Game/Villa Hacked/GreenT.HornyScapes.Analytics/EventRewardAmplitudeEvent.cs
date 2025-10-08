namespace GreenT.HornyScapes.Analytics;

public class EventRewardAmplitudeEvent : AmplitudeEvent
{
	private const string ANALYTIC_EVENT = "calendar_event_reward";

	private const string CALENDAR_ID = "calendar_id";

	private const string EVENT_ID = "event_id";

	private const string REWARDS_COUNT = "rewards_count";

	private const string FREE_TRACK = "free_track";

	private const string PAID_TRACK = "paid_track";

	public EventRewardAmplitudeEvent(int calendarId, int eventId, int rewards_count, int free_track, int paid_track)
		: base("calendar_event_reward")
	{
		((AnalyticsEvent)this).AddEventParams("calendar_id", (object)calendarId);
		((AnalyticsEvent)this).AddEventParams("event_id", (object)eventId);
		((AnalyticsEvent)this).AddEventParams("rewards_count", (object)rewards_count);
		((AnalyticsEvent)this).AddEventParams("free_track", (object)free_track);
		((AnalyticsEvent)this).AddEventParams("paid_track", (object)paid_track);
	}
}
