using System;
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
		_gameStarter = gameStarter;
		_intConstants = intConstants;
		_playerBasics = playerBasics;
		_multiplierManager = multiplierManager;
	}

	public void Initialize()
	{
		IObservable<bool> source = _gameStarter.IsGameReadyToStart.Where((bool status) => status).Take(1);
		(from value in source.SelectMany((bool _) => _multiplierManager.IncreaseEnergyMultipliers.Total.Factor)
			select (int)value into value
			where value >= 0
			select value).Subscribe(delegate(int value)
		{
			_energy.UpdateBoundsInfluenced(_maxEnergy + value, 0);
		}).AddTo(_compositeDisposable);
		(from value in source.SelectMany((bool _) => _multiplierManager.IncreaseEnergyRechargeSpeedMultipliers.Total.Factor)
			select (int)value into value
			where value >= 0
			select value).Subscribe(delegate(int value)
		{
			if (value == 0)
			{
				value = 1;
			}
			_energy.ChangeRestorePeriod(TimeSpan.FromSeconds(_baseRechargeSpeed) / value);
		}).AddTo(_compositeDisposable);
	}

	public void Dispose()
	{
		_compositeDisposable?.Dispose();
	}
}
