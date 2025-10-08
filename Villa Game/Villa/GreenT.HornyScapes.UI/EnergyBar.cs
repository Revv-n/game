using UniRx;
using Zenject;

namespace GreenT.HornyScapes.UI;

public sealed class EnergyBar : EnergyBarBase
{
	private RestoreEnergyPopupOpener _restoreEnergyPopupOpener;

	[Inject]
	public void Init(IPlayerBasics playerBasics, RestoreEnergyPopupOpener restoreEnergyPopupOpener)
	{
		_energy = playerBasics.Energy;
		_restoreEnergyPopupOpener = restoreEnergyPopupOpener;
	}

	protected override void Awake()
	{
		base.Awake();
		_energy.OnUpdate.AsUnitObservable().Subscribe(delegate
		{
			_timerView.Init(_energy.Timer, _timeHelper.UseCombineFormat);
		}).AddTo(this);
	}

	protected override void ShowRestoreEnergyPopup()
	{
		_restoreEnergyPopupOpener.Open();
	}
}
