using System.Collections.Generic;

namespace GreenT.HornyScapes.MiniEvents;

public interface IMiniEventConfigHandler
{
	ConfigType ConfigType { get; }

	IEnumerable<IController> Handle<T>(T mapper, int eventId, int activityId, int calendarId);
}
