using System.Linq;
using GreenT.Model.Collections;

namespace GreenT.HornyScapes.Events;

public class EventMapperProvider : SimpleManager<EventMapper>
{
	public EventMapper GetEventMapper(int id)
	{
		return collection.FirstOrDefault((EventMapper _event) => _event.event_id == id);
	}
}
