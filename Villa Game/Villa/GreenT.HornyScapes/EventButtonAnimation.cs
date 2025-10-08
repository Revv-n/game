using System;
using DG.Tweening;
using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Events;
using StripClub.Model;
using StripClub.Model.Shop;
using StripClub.UI;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes;

public sealed class EventButtonAnimation : MonoView
{
	[SerializeField]
	private Transform _target;

	[SerializeField]
	private CurrencyType _currencyType = CurrencyType.EventXP;

	[SerializeField]
	private CurrencyFlySettingsSO _flySettings;

	[SerializeField]
	private SpriteBezierAnimateSO _bezierSettings;

	[SerializeField]
	private float _offsetDuration = -0.05f;

	private CalendarQueue _calendarQueue;

	private EventSettingsProvider _eventSettingsProvider;

	private ICurrencyProcessor _currencyProcessor;

	private IReadOnlyReactiveProperty<int> _currency;

	private IDisposable _currencyTracker;

	private Sequence _sequence;

	[Inject]
	private void Init(CalendarQueue calendarQueue, EventSettingsProvider eventSettingsProvider, ICurrencyProcessor currencyProcessor)
	{
		_calendarQueue = calendarQueue;
		_eventSettingsProvider = eventSettingsProvider;
		_currencyProcessor = currencyProcessor;
	}

	private void Awake()
	{
		_currency = _currencyProcessor.GetCountReactiveProperty(_currencyType);
		BuildAnimation();
	}

	private void OnEnable()
	{
		CalendarModel activeCalendar = _calendarQueue.GetActiveCalendar(EventStructureType.Event);
		if (activeCalendar == null)
		{
			return;
		}
		GreenT.HornyScapes.Events.Event @event = _eventSettingsProvider.GetEvent(activeCalendar.BalanceId);
		if (@event != null && (@event.GlobalRatingId != 0 || @event.GroupRatingId != 0))
		{
			_currencyTracker = _currency.Skip(1).Subscribe(delegate
			{
				Punch();
			});
		}
	}

	private void OnDisable()
	{
		_currencyTracker?.Dispose();
		_currencyTracker = null;
	}

	private void BuildAnimation()
	{
		_sequence?.Kill();
		Vector3 endValue = Vector3.one * _flySettings.PunchIconScale;
		float interval = _bezierSettings.Duration + _offsetDuration;
		_sequence = DOTween.Sequence().SetAutoKill(autoKillOnCompletion: false).AppendInterval(interval)
			.Append(_target.DOScale(endValue, _flySettings.PunchIconDuration))
			.Append(_target.DOScale(Vector3.one, _flySettings.PunchIconDuration / 2f))
			.OnComplete(delegate
			{
				_sequence.Rewind();
			});
	}

	private void Punch()
	{
		if (_currency.Value > 0)
		{
			_sequence.Restart();
		}
	}
}
