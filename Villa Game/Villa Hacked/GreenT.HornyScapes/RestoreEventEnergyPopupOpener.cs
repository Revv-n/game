using GreenT.UI;

namespace GreenT.HornyScapes;

public class RestoreEventEnergyPopupOpener
{
	private RestoreEventEnergyPopup _restoreEnergyPopup;

	private readonly IWindowsManager _windowsManager;

	public RestoreEventEnergyPopupOpener(IWindowsManager windowsManager)
	{
		_windowsManager = windowsManager;
	}

	public void Open()
	{
		if (!_restoreEnergyPopup)
		{
			_restoreEnergyPopup = _windowsManager.Get<RestoreEventEnergyPopup>();
		}
		_restoreEnergyPopup.Open();
	}
}
