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
		EventDownloadEvent eventDownloadEvent = new EventDownloadEvent(currentEventType, eventId, calendarId, downloadTime);
		((IAnalyticSender<AmplitudeEvent>)(object)amplitude).AddEvent((AmplitudeEvent)eventDownloadEvent);
	}

	public void SendEventEndEvent(EventStructureType currentEventType, int eventId, int calendarId, long duration)
	{
		EventEndEvent eventEndEvent = new EventEndEvent(currentEventType, eventId, calendarId, duration);
		((IAnalyticSender<AmplitudeEvent>)(object)amplitude).AddEvent((AmplitudeEvent)eventEndEvent);
	}
}
