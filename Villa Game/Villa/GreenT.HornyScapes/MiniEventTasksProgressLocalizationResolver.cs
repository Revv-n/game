using System.Collections.Generic;
using GreenT.HornyScapes.MiniEvents;

namespace GreenT.HornyScapes;

public sealed class MiniEventTasksProgressLocalizationResolver : MiniEventLocalizationKeyResolver<bool>
{
	public MiniEventTasksProgressLocalizationResolver()
	{
		_localizationKeys = new Dictionary<bool, string>
		{
			{ true, "ui.minievent.task.progress.complete" },
			{ false, "ui.minievent.task.progress" }
		};
	}
}
