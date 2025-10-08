using System;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.Lootboxes;
using GreenT.Localizations;
using Merge.Meta.RoomObjects;
using StripClub.Model.Shop;
using StripClub.UI;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.BattlePassSpace.UI.MetaWindow;

public class EventBattlePassButtonView : MonoView
{
	[SerializeField]
	private TMP_Text _levelField;

	[SerializeField]
	private EventBattlePassButtonSubscription _subscription;

	[SerializeField]
	private Image _icon;

	[SerializeField]
	private EventBattlePassNotify _notification;

	[SerializeField]
	private Image _levelHolderBackground;

	[SerializeField]
	private string _maxLevelKey;

	private BattlePassMapperProvider _mapperProvider;

	private ICurrencyProcessor _currencyProcessor;

	private LocalizationService _localizationService;

	private BattlePass _battlePass;

	private IReadOnlyReactiveProperty<int> _currencyTarget;

	private IDisposable _calendarStateStream;

	private IDisposable _currencyTargetStream;

	[Inject]
	private void Construct(BattlePassMapperProvider mapperProvider, ICurrencyProcessor currencyProcessor, LocalizationService localizationService)
	{
		_mapperProvider = mapperProvider;
		_currencyProcessor = currencyProcessor;
		_localizationService = localizationService;
	}

	public void Set(CalendarModel eventCalendarModel, BattlePass battlePass)
	{
		_battlePass = battlePass;
		_notification.Set(_battlePass);
		_calendarStateStream?.Dispose();
		_calendarStateStream = eventCalendarModel.CalendarState.Subscribe(SetState);
		TrackPoints();
		Show();
	}

	public void Hide()
	{
		base.gameObject.SetActive(value: false);
	}

	public void Show()
	{
		base.gameObject.SetActive(value: true);
	}

	private void OnDestroy()
	{
		_calendarStateStream?.Dispose();
	}

	private void SetState(EntityStatus status)
	{
		switch (status)
		{
		case EntityStatus.Blocked:
			SetPreview();
			break;
		case EntityStatus.InProgress:
			SetStart();
			break;
		case EntityStatus.Complete:
			SetComplete();
			break;
		default:
			throw new Exception().SendException($"{GetType().Name}: doesn't have behaviour for {status} int = {status}");
		case EntityStatus.Rewarded:
			break;
		}
	}

	private void SetPreview()
	{
		_levelHolderBackground.gameObject.SetActive(value: false);
		_levelField.transform.gameObject.SetActive(value: false);
	}

	private void SetStart()
	{
		_levelHolderBackground.gameObject.SetActive(value: true);
		_levelField.transform.gameObject.SetActive(value: true);
		_icon.sprite = _battlePass.CurrentViewSettings.LevelButton;
	}

	private void SetComplete()
	{
		_levelHolderBackground.gameObject.SetActive(value: false);
	}

	private void TrackPoints()
	{
		string text = _mapperProvider.GetEventMapper(_battlePass.ID).bp_resource;
		if (string.IsNullOrEmpty(text))
		{
			text = "bp_points";
		}
		SelectorTools.GetResourceEnumValueByConfigKey(text, out var currency);
		if (_currencyProcessor.TryGetCountReactiveProperty(currency, out _currencyTarget))
		{
			_currencyTargetStream?.Dispose();
			_currencyTargetStream = _currencyTarget.Subscribe(UpdateLevel);
		}
	}

	private void UpdateLevel(int points)
	{
		int levelForPoints = _battlePass.GetLevelForPoints(points);
		int pointsForMaxLevel = _battlePass.GetPointsForMaxLevel();
		_levelField.text = ((points < pointsForMaxLevel) ? levelForPoints.ToString() : _localizationService.Text(_maxLevelKey));
		_battlePass.Data.LevelInfo.AddPoints(points - _battlePass.Data.LevelInfo.Points.Value);
	}
}
