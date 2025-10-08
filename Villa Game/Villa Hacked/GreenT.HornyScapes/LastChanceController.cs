using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Events;
using StripClub.Extensions;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes;

public sealed class LastChanceController : IDisposable
{
	private IDisposable _timer;

	private readonly LastChanceManager _manager;

	private readonly IClock _clock;

	private readonly List<LastChance> _activatedLastChances;

	private readonly Dictionary<LastChanceType, BaseLastChanceStrategy> _strategies;

	private readonly Subject<LastChance> _onLastChanceActivated;

	private readonly Subject<LastChance> _onLastChanceDeactivated;

	public IObservable<LastChance> OnLastChanceActivated => Observable.AsObservable<LastChance>((IObservable<LastChance>)_onLastChanceActivated);

	public IObservable<LastChance> OnLastChanceDeactivated => Observable.AsObservable<LastChance>((IObservable<LastChance>)_onLastChanceDeactivated);

	[Inject]
	public LastChanceController(LastChanceManager manager, LastChanceRatingsStrategy lastChanceRatingsStrategy, LastChanceEventBattlePassStrategy lastChanceEventBattlePassStrategy, IClock clock)
	{
		_manager = manager;
		_clock = clock;
		_activatedLastChances = new List<LastChance>();
		_onLastChanceActivated = new Subject<LastChance>();
		_onLastChanceDeactivated = new Subject<LastChance>();
		_strategies = new Dictionary<LastChanceType, BaseLastChanceStrategy>
		{
			{
				LastChanceType.EventRating,
				lastChanceRatingsStrategy
			},
			{
				LastChanceType.EventBP,
				lastChanceEventBattlePassStrategy
			}
		};
	}

	public void Init()
	{
		_timer = ObservableExtensions.Subscribe<long>(Observable.Interval(TimeSpan.FromSeconds(1.0)), (Action<long>)delegate
		{
			HandleLastChances();
		});
	}

	public void Dispose()
	{
		_timer?.Dispose();
	}

	public void HandleLastChance(LastChance lastChance)
	{
		if (lastChance.LastChanceType != 0)
		{
			_strategies[lastChance.LastChanceType].Execute(lastChance);
		}
	}

	private void HandleLastChances()
	{
		if (!_manager.Collection.Any())
		{
			return;
		}
		long num = _clock.GetTime().ConvertToUnixTimestamp();
		for (int i = 0; i < _manager.ListCollection.Count; i++)
		{
			LastChance lastChance = _manager.ListCollection[i];
			if (lastChance.LastChanceType == LastChanceType.None)
			{
				_manager.Remove(lastChance);
			}
			else if (lastChance.EndDate < num)
			{
				_strategies[lastChance.LastChanceType].Stop(lastChance);
				OnStopped(lastChance);
			}
			else if (!_activatedLastChances.Contains(lastChance) && lastChance.StartDate < num && num < lastChance.EndDate)
			{
				BaseLastChanceStrategy baseLastChanceStrategy = _strategies[lastChance.LastChanceType];
				baseLastChanceStrategy.Init(lastChance, OnInitFinished);
				baseLastChanceStrategy.Stopped += OnStopped;
			}
		}
	}

	private void OnInitFinished(LastChance lastChance)
	{
		_activatedLastChances.Add(lastChance);
		_onLastChanceActivated.OnNext(lastChance);
	}

	private void OnStopped(LastChance lastChance)
	{
		_onLastChanceDeactivated.OnNext(lastChance);
		_manager.Remove(lastChance);
		_activatedLastChances.Remove(lastChance);
	}
}
