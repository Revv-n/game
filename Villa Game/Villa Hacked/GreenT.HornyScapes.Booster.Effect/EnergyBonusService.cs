using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Constants;
using GreenT.Multiplier;
using StripClub.Extensions;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Booster.Effect;

public class EnergyBonusService : IInitializable, IDisposable
{
	private readonly GameStarter _gameStarter;

	private readonly IPlayerBasics _playerBasics;

	private readonly MultiplierManager _multiplierManager;

	private bool _isInited;

	private readonly IConstants<int> _intConstants;

	private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

	private int _maxEnergy => _intConstants["max_energy"];

	private int _baseRechargeSpeed => _intConstants["time_energy_restore"];

	private RestorableValue<int> _energy => _playerBasics.Energy;

	public EnergyBonusService(MultiplierManager multiplierManager, IPlayerBasics playerBasics, GameStarter gameStarter, BoosterStorage boosterStorage, IConstants<int> intConstants)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		_gameStarter = gameStarter;
		_intConstants = intConstants;
		_playerBasics = playerBasics;
		_multiplierManager = multiplierManager;
	}

	public void Initialize()
	{
		IObservable<bool> observable = Observable.Take<bool>(Observable.Where<bool>((IObservable<bool>)_gameStarter.IsGameReadyToStart, (Func<bool, bool>)((bool status) => status)), 1);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<int>(Observable.Where<int>(Observable.Select<double, int>(Observable.SelectMany<bool, double>(observable, (Func<bool, IObservable<double>>)((bool _) => (IObservable<double>)_multiplierManager.IncreaseEnergyMultipliers.Total.Factor)), (Func<double, int>)((double value) => (int)value)), (Func<int, bool>)((int value) => value >= 0)), (Action<int>)delegate(int value)
		{
			_energy.UpdateBoundsInfluenced(_maxEnergy + value, 0);
		}), (ICollection<IDisposable>)_compositeDisposable);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<int>(Observable.Where<int>(Observable.Select<double, int>(Observable.SelectMany<bool, double>(observable, (Func<bool, IObservable<double>>)((bool _) => (IObservable<double>)_multiplierManager.IncreaseEnergyRechargeSpeedMultipliers.Total.Factor)), (Func<double, int>)((double value) => (int)value)), (Func<int, bool>)((int value) => value >= 0)), (Action<int>)delegate(int value)
		{
			if (value == 0)
			{
				value = 1;
			}
			_energy.ChangeRestorePeriod(TimeSpan.FromSeconds(_baseRechargeSpeed) / (double)value);
		}), (ICollection<IDisposable>)_compositeDisposable);
	}

	public void Dispose()
	{
		CompositeDisposable compositeDisposable = _compositeDisposable;
		if (compositeDisposable != null)
		{
			compositeDisposable.Dispose();
		}
	}
}
