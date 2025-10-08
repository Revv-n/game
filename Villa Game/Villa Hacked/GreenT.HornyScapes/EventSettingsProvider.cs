using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Events;
using GreenT.Model.Collections;

namespace GreenT.HornyScapes;

public sealed class EventSettingsProvider : SimpleManager<Event>
{
	public Event GetEvent(int eventId)
	{
		return Collection.FirstOrDefault((Event x) => x.EventId == eventId);
	}

	public IReadOnlyList<CalendarModel> GetActiveModels(IEnumerable<CalendarModel> models)
	{
		return models.Where((CalendarModel m) => Collection.Any((Event c) => c.EventId == m.BalanceId)).ToList();
	}

	public bool TryGetEvent(int id, out Event @event)
	{
		@event = GetEvent(id);
		return @event != null;
	}
}
