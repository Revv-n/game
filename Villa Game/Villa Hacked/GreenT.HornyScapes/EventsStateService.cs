using System.Linq;
using GreenT.HornyScapes.Events;

namespace GreenT.HornyScapes;

public sealed class EventsStateService : BaseStateService<Event>
{
	private readonly EventSettingsProvider _eventSettingsProvider;

	public EventsStateService(CalendarQueue calendarQueue, EventSettingsProvider eventSettingsProvider)
		: base(calendarQueue, EventStructureType.Event)
	{
		_eventSettingsProvider = eventSettingsProvider;
	}

	protected override Event GetModel(int eventId, int calendarId)
	{
		return _eventSettingsProvider.Collection.FirstOrDefault((Event m) => m.EventId == eventId && m.CalendarId == calendarId);
	}
}
