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
		AddEventParams("event_type", currentEventType);
		AddEventParams("event_id", eventId);
		AddEventParams("calendar_id", calendarId);
		AddEventParams("download_time", downloadTime);
	}
}
