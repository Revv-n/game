using GreenT.UI;

namespace GreenT.HornyScapes.Maintenance.UI;

public class MaintenanceWindowOpener : BaseWindowOpener
{
	public MaintenanceWindowOpener(IWindowsManager windowsManager, WindowGroupID windowID)
		: base(windowsManager, windowID)
	{
	}
}
