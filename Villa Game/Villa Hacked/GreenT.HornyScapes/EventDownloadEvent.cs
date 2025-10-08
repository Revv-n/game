using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.Events;

namespace GreenT.HornyScapes;

public class EventDownloadEvent : AmplitudeEvent
{
	private const string EventTypeKey = "event_download";

	private const string CurrentEventTypeKey = "event_type";

	private const string EventIdKey = "event_id";

	private const string CalendarIdKey = "calendar_id";

	private const string DownloadTimeKey = "download_time";

	public EventDownloadEvent(EventStructureType currentEventType, int eventId, int calendarId, string downloadTime)
		: base("event_download")
	{
		((AnalyticsEvent)this).AddEventParams("event_type", (object)currentEventType);
		((AnalyticsEvent)this).AddEventParams("event_id", (object)eventId);
		((AnalyticsEvent)this).AddEventParams("calendar_id", (object)calendarId);
		((AnalyticsEvent)this).AddEventParams("download_time", (object)downloadTime);
	}
}
