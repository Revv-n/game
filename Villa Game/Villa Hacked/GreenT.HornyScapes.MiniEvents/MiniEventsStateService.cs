using System.Linq;
using GreenT.HornyScapes.Events;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class MiniEventsStateService : BaseStateService<MiniEvent>
{
	private readonly MiniEventSettingsProvider _miniEventSettingsProvider;

	public MiniEventsStateService(CalendarQueue calendarQueue, MiniEventSettingsProvider miniEventSettingsProvider)
		: base(calendarQueue, EventStructureType.Mini)
	{
		_miniEventSettingsProvider = miniEventSettingsProvider;
	}

	protected override MiniEvent GetModel(int eventId, int calendarId)
	{
		return _miniEventSettingsProvider.Collection.FirstOrDefault((MiniEvent m) => m.EventId == eventId && m.CalendarId == calendarId);
	}
}
