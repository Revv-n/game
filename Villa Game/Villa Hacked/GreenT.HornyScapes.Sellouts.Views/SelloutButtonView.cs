using System;
using System.Linq;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.Sellouts.Models;
using GreenT.HornyScapes.Sellouts.Services;
using GreenT.Localizations;
using Merge.Meta.RoomObjects;
using StripClub.Extensions;
using StripClub.UI;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Sellouts.Views;

public class SelloutButtonView : MonoView, IInitializable
{
	private const string SelloutNameKey = "ui.icon.sellout_{0}.name";

	[SerializeField]
	private Image _icon;

	[SerializeField]
	private TMP_Text _name;

	[SerializeField]
	private MonoTimer _timerView;

	private TimeHelper _timeHelper;

	private SelloutStateManager _selloutStateManager;

	private CalendarManager _calendarManager;

	private LocalizationService _localizationService;

	private IDisposable _calendarStateStream;

	private IDisposable _localizationDisposable;

	private readonly CompositeDisposable _subscriptions = new CompositeDisposable();

	[Inject]
	public void Init(TimeHelper timeHelper, SelloutStateManager selloutStateManager, CalendarManager calendarManager, LocalizationService localizationService)
	{
		_timeHelper = timeHelper;
		_selloutStateManager = selloutStateManager;
		_calendarManager = calendarManager;
		_localizationService = localizationService;
	}

	public void Initialize()
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Sellout>(_selloutStateManager.Activated, (Action<Sellout>)delegate(Sellout sellout)
		{
			Set(sellout);
		}), (Component)this);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Sellout>(_selloutStateManager.Deactivated, (Action<Sellout>)delegate
		{
			base.gameObject.SetActive(value: false);
		}), (Component)this);
	}

	private void Awake()
	{
		base.gameObject.SetActive(value: false);
	}

	private void OnDestroy()
	{
		_subscriptions.Dispose();
		_calendarStateStream?.Dispose();
		_localizationDisposable?.Dispose();
	}

	private void Set(Sellout sellout)
	{
		_icon.sprite = sellout.BundleData.IconSprite;
		string key = $"ui.icon.sellout_{sellout.ID}.name";
		_localizationDisposable?.Dispose();
		_localizationDisposable = ObservableExtensions.Subscribe<string>((IObservable<string>)_localizationService.ObservableText(key), (Action<string>)delegate(string text)
		{
			_name.text = text;
		});
		_timerView.gameObject.SetActive(value: false);
		CalendarModel calendarModel = _calendarManager.Collection.FirstOrDefault((CalendarModel calendarModel) => calendarModel.EventType == EventStructureType.Sellout && calendarModel.BalanceId == sellout.ID);
		if (calendarModel != null)
		{
			_calendarStateStream?.Dispose();
			_calendarStateStream = ObservableExtensions.Subscribe<EntityStatus>((IObservable<EntityStatus>)calendarModel.CalendarState, (Action<EntityStatus>)delegate
			{
				Activate(calendarModel);
			});
		}
	}

	private void Activate(CalendarModel calendarModel)
	{
		bool flag = calendarModel.CalendarState.Value != EntityStatus.Blocked;
		base.gameObject.SetActive(flag);
		if (flag)
		{
			SetTimer(calendarModel);
		}
	}

	private void SetTimer(CalendarModel calendarModel)
	{
		bool value = calendarModel.Duration.IsActive.Value;
		_timerView.Init(calendarModel.Duration, _timeHelper.UseCombineFormat);
		_timerView.gameObject.SetActive(value);
	}
}
