using System;
using UniRx;
using UnityEngine;
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
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Unit>(Observable.AsUnitObservable<Unit>((IObservable<Unit>)_energy.OnUpdate), (Action<Unit>)delegate
		{
			_timerView.Init(_energy.Timer, _timeHelper.UseCombineFormat);
		}), (Component)this);
	}

	protected override void ShowRestoreEnergyPopup()
	{
		_restoreEnergyPopupOpener.Open();
	}
}
