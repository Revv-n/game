using System.Collections.Generic;
using StripClub.Model;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class MiniEventSpendCurrencyTasksLocalizationResolver : MiniEventLocalizationKeyResolver<CurrencyType>
{
	public MiniEventSpendCurrencyTasksLocalizationResolver()
	{
		_localizationKeys = new Dictionary<CurrencyType, string>
		{
			{
				CurrencyType.Soft,
				"ui.minievent.task.spendSoft.descr"
			},
			{
				CurrencyType.Hard,
				"ui.minievent.task.spendHard.descr"
			},
			{
				CurrencyType.Energy,
				"ui.minievent.task.spendEnergy.descr"
			},
			{
				CurrencyType.MiniEvent,
				"ui.minievent.task.spendMinievent{0}.descr"
			}
		};
	}
}
