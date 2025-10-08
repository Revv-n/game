using System.Collections.Generic;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class MiniEventActivityTabsLocalizationResolver : MiniEventLocalizationKeyResolver<TabType>
{
	public MiniEventActivityTabsLocalizationResolver()
	{
		_localizationKeys = new Dictionary<TabType, string>
		{
			{
				TabType.Task,
				"ui.minievent.questtab_{0}.name"
			},
			{
				TabType.Shop,
				"ui.minievent.shoptab_{0}.name"
			}
		};
	}
}
