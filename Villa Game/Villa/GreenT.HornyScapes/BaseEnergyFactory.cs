using System;
using GreenT.Data;
using GreenT.HornyScapes.Constants;
using StripClub.Extensions;
using Zenject;

namespace GreenT.HornyScapes;

public abstract class BaseEnergyFactory : PlayerRestorableVariablesFactory, IFactory<RestorableValue<int>>, IFactory
{
	private readonly IConstants<int> _intConstants;

	private readonly ISaver _saver;

	protected RestorableValue<int> _energy;

	protected RestorableEventEnergyValue<int> _eventEnergy;

	protected string _startEnergyKey;

	protected string _timeEnergyRestoreKey;

	protected string _amountEnergyRestoreKey;

	protected string _maxEnergyKey;

	protected string _saveKey;

	public BaseEnergyFactory(IClock clock, IConstants<int> intConstants, ISaver saver, EnergyLoadContainer loadContainer)
		: base(clock, loadContainer)
	{
		_intConstants = intConstants;
		_saver = saver;
	}

	public RestorableValue<int> Create()
	{
		int num = _intConstants[_startEnergyKey];
		int num2 = _intConstants[_timeEnergyRestoreKey];
		int num3 = _intConstants[_amountEnergyRestoreKey];
		int maxValue = _intConstants[_maxEnergyKey];
		if (_energy == null)
		{
			_energy = Create(num, num2, num3, maxValue, _saveKey);
			_saver.Add(_energy);
		}
		else
		{
			Initialize(num, num2, num3, maxValue);
		}
		return _energy;
	}

	public RestorableEventEnergyValue<int> CreateEventEnergy()
	{
		int num = _intConstants[_startEnergyKey];
		int num2 = _intConstants[_timeEnergyRestoreKey];
		int num3 = _intConstants[_amountEnergyRestoreKey];
		int maxValue = _intConstants[_maxEnergyKey];
		if (_eventEnergy == null)
		{
			_eventEnergy = CreateEventEnergy(num, num2, num3, maxValue, _saveKey);
			_saver.Add(_eventEnergy);
		}
		else
		{
			InitializeEventEnergy(num, num2, num3, maxValue);
		}
		return _eventEnergy;
	}

	private void Initialize(int value, int restoreTime, int amountPerTick, int maxValue)
	{
		_energy.SetForce(value);
		_energy.UpdateBounds(maxValue, 0);
		_energy.AmountPerTick = amountPerTick;
		_energy.RestorePeriod = TimeSpan.FromSeconds(restoreTime);
	}

	private void InitializeEventEnergy(int value, int restoreTime, int amountPerTick, int maxValue)
	{
		_eventEnergy.SetForce(value);
		_eventEnergy.UpdateBounds(maxValue, 0);
		_eventEnergy.AmountPerTick = amountPerTick;
		_eventEnergy.RestorePeriod = TimeSpan.FromSeconds(restoreTime);
	}
}
