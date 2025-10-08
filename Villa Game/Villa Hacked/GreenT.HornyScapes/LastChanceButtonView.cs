using System;
using StripClub.Extensions;
using StripClub.UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes;

public abstract class LastChanceButtonView : MonoView
{
	[SerializeField]
	protected Image _icon;

	[SerializeField]
	private Button _lastChanceButton;

	[SerializeField]
	private MonoTimer _timerView;

	[SerializeField]
	private LastChanceType[] _handleLastChanceTypes;

	protected LastChance _currentLastChance;

	protected LastChanceController _lastChanceController;

	private TimeHelper _timeHelper;

	private TimeInstaller.TimerCollection _timers;

	private IClock _clock;

	private GenericTimer _timer;

	private IDisposable _lastChanceActivatedDisposable;

	private IDisposable _lastChanceDeactivatedDisposable;

	[Inject]
	private void Init(LastChanceController lastChanceController, TimeHelper timeHelper, [InjectOptional] TimeInstaller.TimerCollection timers, IClock clock)
	{
		_lastChanceController = lastChanceController;
		_timeHelper = timeHelper;
		_timers = timers;
		_clock = clock;
		_lastChanceActivatedDisposable = ObservableExtensions.Subscribe<LastChance>(_lastChanceController.OnLastChanceActivated, (Action<LastChance>)OnLastChanceActivated);
		_lastChanceDeactivatedDisposable = ObservableExtensions.Subscribe<LastChance>(_lastChanceController.OnLastChanceDeactivated, (Action<LastChance>)OnLastChanceDeactivated);
		_lastChanceButton.onClick.AddListener(OnLastChanceButtonClick);
		_timer = new GenericTimer(TimeSpan.Zero);
		_timers.Add(_timer);
	}

	private void OnDestroy()
	{
		_lastChanceActivatedDisposable?.Dispose();
		_lastChanceDeactivatedDisposable?.Dispose();
		_lastChanceButton.onClick.RemoveListener(OnLastChanceButtonClick);
	}

	protected abstract void Activate();

	private void StartTimer()
	{
		long num = _clock.GetTime().ConvertToUnixTimestamp();
		TimeSpan timeLeft = TimeSpan.FromSeconds(_currentLastChance.EndDate - num);
		_timer.Start(timeLeft);
		_timerView.Init(_timer, (TimeSpan timeSpan) => _timeHelper.UseCombineFormat(timeSpan));
	}

	private void OnLastChanceButtonClick()
	{
		_lastChanceController.HandleLastChance(_currentLastChance);
	}

	private void OnLastChanceActivated(LastChance lastChance)
	{
		if (_currentLastChance != null)
		{
			return;
		}
		for (int i = 0; i < _handleLastChanceTypes.Length; i++)
		{
			if (_handleLastChanceTypes[i] == lastChance.LastChanceType)
			{
				_currentLastChance = lastChance;
				Activate();
				StartTimer();
				_lastChanceButton.gameObject.SetActive(value: true);
				break;
			}
		}
	}

	private void OnLastChanceDeactivated(LastChance lastChance)
	{
		if (_currentLastChance == lastChance)
		{
			_lastChanceButton.gameObject.SetActive(value: false);
			_currentLastChance = null;
		}
	}
}
