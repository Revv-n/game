using System;
using GreenT.HornyScapes.Constants;
using Zenject;

namespace GreenT.HornyScapes.ToolTips;

public class ToolTipUIImageOpener : OnPointerDownToolTipOpener<ToolTipImageSettings, ToolTipImageView>
{
	[Inject]
	private void Init(IConstants<int> intConstants)
	{
		showTime = TimeSpan.FromSeconds(intConstants["phrase_hiring"]);
	}
}
