using System;
using StripClub.UI;

namespace GreenT.HornyScapes.ToolTips;

public class ToolTipUIOpener<TSettings, TView> : OnPointerDownToolTipOpener<TSettings, TView> where TSettings : ToolTipSettings where TView : MonoView<TSettings>
{
	private const float MAXTIME = 999f;

	public override void Awake()
	{
		base.Awake();
		showTime = TimeSpan.FromSeconds(999.0);
	}
}
public class ToolTipUIOpener : ToolTipUIOpener<ToolTipUISettings, UIToolTipView>
{
	private object[] arguments;

	public void SetArguments(params object[] arguments)
	{
		this.arguments = arguments;
	}

	public override void OpenToolTip(ToolTipUISettings settings)
	{
		base.OpenToolTip(settings);
		if (arguments != null && arguments.Length != 0)
		{
			view.SetArguments(arguments);
		}
	}
}
