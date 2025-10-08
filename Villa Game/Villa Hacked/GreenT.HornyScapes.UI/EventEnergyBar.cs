using StripClub.Extensions;
using Zenject;

namespace GreenT.HornyScapes.UI;

public sealed class EventEnergyBar : EnergyBarBase
{
	private RestoreEventEnergyPopupOpener _restoreEnergyPopupOpener;

	private RestorableEventEnergyValue<int> _energyEvent;

	[Inject]
	public void Init(IPlayerBasics playerBasics, RestoreEventEnergyPopupOpener restoreEnergyPopupOpener)
	{
		_energyEvent = playerBasics.EventEnergy;
		_restoreEnergyPopupOpener = restoreEnergyPopupOpener;
	}

	protected override void Awake()
	{
		_plus.onClick.AddListener(ShowRestoreEnergyPopup);
		_timerView.Init(_energyEvent.Timer, _timeHelper.UseCombineFormat);
	}

	protected override void ShowRestoreEnergyPopup()
	{
		_restoreEnergyPopupOpener.Open();
	}
}
