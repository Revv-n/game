using System.Linq;
using GreenT.UI;
using StripClub.Model.Shop;

namespace GreenT.HornyScapes;

public class RestoreEnergyPopupOpener
{
	private RestoreEnergyPopup _restoreEnergyPopup;

	private readonly IWindowsManager _windowsManager;

	private readonly LotManager _lotManager;

	private readonly RestoreEnergyPopupSetter _restoreEnergyPopupSetter;

	public RestoreEnergyPopupOpener(IWindowsManager windowsManager, RestoreEnergyPopupSetter restoreEnergyPopupSetter, LotManager lotManager)
	{
		_lotManager = lotManager;
		_windowsManager = windowsManager;
		_restoreEnergyPopupSetter = restoreEnergyPopupSetter;
	}

	public void Open()
	{
		if (!_restoreEnergyPopup)
		{
			_restoreEnergyPopup = _windowsManager.Get<RestoreEnergyPopup>();
		}
		_restoreEnergyPopupSetter.SetSubscription(_lotManager.GetLot<SubscriptionLot>().Any((SubscriptionLot lot) => lot.Locker.IsOpen.Value));
		_restoreEnergyPopup.Open();
	}
}
