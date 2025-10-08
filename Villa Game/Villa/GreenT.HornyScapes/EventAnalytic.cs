using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.Events;

namespace GreenT.HornyScapes;

public class EventAnalytic : BaseAnalytic
{
	public EventAnalytic(IAmplitudeSender<AmplitudeEvent> amplitude)
		: base(amplitude)
	{
	}

	public void SendEventDownloadEvent(EventStructureType currentEventType, int eventId, int calendarId, string downloadTime)
	{
		EventDownloadEvent analyticsEvent = new EventDownloadEvent(currentEventType, eventId, calendarId, downloadTime);
		amplitude.AddEvent(analyticsEvent);
	}

	public void SendEventEndEvent(EventStructureType currentEventType, int eventId, int calendarId, long duration)
	{
		EventEndEvent analyticsEvent = new EventEndEvent(currentEventType, eventId, calendarId, duration);
		amplitude.AddEvent(analyticsEvent);
	}
}
