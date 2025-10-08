using System;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Events;
using StripClub.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes;

public sealed class EventRatingButtonView : MonoView
{
	[SerializeField]
	private TMP_Text _place;

	[SerializeField]
	private Image _icon;

	[SerializeField]
	private Sprite _groupIcon;

	[SerializeField]
	private Sprite _globalIcon;

	[SerializeField]
	private EventRatingWindowView _eventRatingWindowView;

	[SerializeField]
	private Button _ratingButton;

	[SerializeField]
	private GameObject _root;

	private CalendarQueue _calendarQueue;

	private EventSettingsProvider _eventSettingsProvider;

	private TournamentPointsStorage _tournamentPointsStorage;

	private IDisposable _startDisposable;

	private IDisposable _endDisposable;

	private const string ZERO_PLACE = "-";

	[Inject]
	private void Init(CalendarQueue calendarQueue, EventSettingsProvider eventSettingsProvider, TournamentPointsStorage tournamentPointsStorage)
	{
		_calendarQueue = calendarQueue;
		_eventSettingsProvider = eventSettingsProvider;
		_tournamentPointsStorage = tournamentPointsStorage;
	}

	private void Awake()
	{
		_eventRatingWindowView.LeaderboardUpdated += OnLeaderboardUpdated;
		_ratingButton.onClick.AddListener(OnRatingButtonClick);
	}

	private void OnEnable()
	{
		CalendarModel activeCalendar = _calendarQueue.GetActiveCalendar(EventStructureType.Event);
		if (activeCalendar == null)
		{
			return;
		}
		Event @event = _eventSettingsProvider.GetEvent(activeCalendar.BalanceId);
		if (@event != null)
		{
			if (@event.GlobalRatingId != 0)
			{
				_eventRatingWindowView.InitGlobal(activeCalendar.BalanceId, activeCalendar.UniqID, @event.GlobalRatingId);
			}
			if (@event.GroupRatingId != 0)
			{
				_eventRatingWindowView.InitGroup(activeCalendar.BalanceId, activeCalendar.UniqID, @event.GroupRatingId);
			}
			if (@event.GlobalRatingId != 0 || @event.GroupRatingId != 0)
			{
				_eventRatingWindowView.TryActivateElements(@event.GlobalRatingId, @event.GroupRatingId, isUpdatable: true);
				_eventRatingWindowView.InitDescription(activeCalendar.BalanceId);
				_eventRatingWindowView.InitButtons(@event.GlobalRatingId);
				_eventRatingWindowView.InitBackground(@event.Bundle.Type);
			}
			_root.SetActive(@event.GlobalRatingId != 0 || @event.GroupRatingId != 0);
		}
	}

	private void OnDestroy()
	{
		_eventRatingWindowView.LeaderboardUpdated -= OnLeaderboardUpdated;
		_ratingButton.onClick.RemoveListener(OnRatingButtonClick);
		_startDisposable?.Dispose();
		_endDisposable?.Dispose();
	}

	private void OnLeaderboardUpdated(int globalPlace, int groupPlace)
	{
		bool flag = globalPlace == 0;
		bool flag2 = groupPlace == 0;
		Sprite sprite;
		int num;
		if (flag || flag2)
		{
			sprite = (flag ? _groupIcon : _globalIcon);
			num = (flag ? groupPlace : globalPlace);
		}
		else if (groupPlace == 1)
		{
			sprite = _globalIcon;
			num = globalPlace;
		}
		else
		{
			sprite = _groupIcon;
			num = groupPlace;
		}
		_icon.sprite = sprite;
		_place.text = ((num == 0) ? "-" : $"{num}");
	}

	private void OnRatingButtonClick()
	{
		_eventRatingWindowView.Open();
	}
}
