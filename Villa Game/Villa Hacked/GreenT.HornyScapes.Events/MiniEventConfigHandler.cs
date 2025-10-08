using System.Collections.Generic;
using GreenT.HornyScapes.MiniEvents;

namespace GreenT.HornyScapes.Events;

public sealed class MiniEventConfigHandler
{
	private readonly Dictionary<GreenT.HornyScapes.MiniEvents.ConfigType, IMiniEventConfigHandler> _configHandlers;

	public MiniEventConfigHandler(MiniEventActivityConfigHandler activityConfigHandler, MiniEventRatingConfigHandler miniEventRatingConfigHandler)
	{
		_configHandlers = new Dictionary<GreenT.HornyScapes.MiniEvents.ConfigType, IMiniEventConfigHandler>
		{
			{
				GreenT.HornyScapes.MiniEvents.ConfigType.Activity,
				activityConfigHandler
			},
			{
				GreenT.HornyScapes.MiniEvents.ConfigType.Rating,
				miniEventRatingConfigHandler
			}
		};
	}

	public IEnumerable<IController> Handle<T>(GreenT.HornyScapes.MiniEvents.ConfigType configType, T mapper, int eventId, int activityId, int calendarId)
	{
		if (!_configHandlers.ContainsKey(configType))
		{
			return null;
		}
		return _configHandlers[configType].Handle(mapper, eventId, activityId, calendarId);
	}
}
