using System.Collections.Generic;
using StripClub.Model;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class MiniEventAddCurrencyTasksLocalizationResolver : MiniEventLocalizationKeyResolver<CurrencyType>
{
	public MiniEventAddCurrencyTasksLocalizationResolver()
	{
		_localizationKeys = new Dictionary<CurrencyType, string>
		{
			{
				CurrencyType.Soft,
				"ui.minievent.task.getSoft.descr"
			},
			{
				CurrencyType.Hard,
				"ui.minievent.task.getHard.descr"
			},
			{
				CurrencyType.MiniEvent,
				"ui.minievent.task.addMinievent{0}.descr"
			},
			{
				CurrencyType.EventXP,
				"ui.eventXP.task.addEventXP{0}.descr"
			},
			{
				CurrencyType.Event,
				"ui.event.task.addEvent{0}.descr"
			},
			{
				CurrencyType.LovePoints,
				"ui.event.task.giveLovePoints.descr"
			}
		};
	}
}
