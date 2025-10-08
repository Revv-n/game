using System;
using GreenT.HornyScapes.Constants;
using GreenT.HornyScapes.ToolTips;
using Zenject;

namespace GreenT.HornyScapes;

public class ToolTipOpenerGirlShow : GirlToolTipOpener
{
	[Inject]
	private void Init(IConstants<int> intConstants)
	{
		showTime = TimeSpan.FromSeconds(intConstants["phrase_hiring"]);
	}
}
