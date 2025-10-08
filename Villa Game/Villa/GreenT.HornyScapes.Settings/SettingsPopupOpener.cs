using GreenT.UI;

namespace GreenT.HornyScapes.Settings;

public class SettingsPopupOpener : BaseWindowOpener
{
	public SettingsPopupOpener(IWindowsManager windowsManager, WindowGroupID windowID)
		: base(windowsManager, windowID)
	{
	}
}
