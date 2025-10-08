using System;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.Lootboxes;
using GreenT.UI;
using StripClub.Model.Shop;
using StripClub.UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.BattlePassSpace.UI.MetaWindow;

public class EventBattlePassButtonSubscription : MonoView
{
	[SerializeField]
	private Button _button;

	[SerializeField]
	private EventBattlePassButtonView _eventBattlePassButtonView;

	private CalendarQueue _calendarQueue;

	private BattlePassSettingsProvider _battlePassSettingsProvider;

	private IWindowsManager _windowsManager;

	private BattlePassMapperProvider _mapperProvider;

	private ICurrencyProcessor _currencyProcessor;

	private CalendarModel _eventCalendarModel;

	private BattlePass _battlePass;

	private BattlePassMetaWindow _battlePassMetaWindow;

	private IDisposable _calendarStream;

	private IDisposable _buttonClickStream;

	[Inject]
	public void Construct(CalendarQueue calendarQueue, BattlePassSettingsProvider battlePassSettingsProvider, IWindowsManager windowsManager, BattlePassMapperProvider mapperProvider, ICurrencyProcessor currencyProcessor)
	{
		_calendarQueue = calendarQueue;
		_battlePassSettingsProvider = battlePassSettingsProvider;
		_windowsManager = windowsManager;
		_mapperProvider = mapperProvider;
		_currencyProcessor = currencyProcessor;
	}

	public void CheckBattlePass()
	{
		CalendarModel activeCalendar = _calendarQueue.GetActiveCalendar(EventStructureType.Event);
		int bp_id = (activeCalendar.EventMapper as EventMapper).bp_id;
		if (!_battlePassSettingsProvider.TryGetBattlePass(bp_id, out var battlePass))
		{
			_eventBattlePassButtonView.Hide();
			return;
		}
		Set(activeCalendar, battlePass);
		_eventBattlePassButtonView.Show();
	}

	private void Set(CalendarModel calendarModel, BattlePass battlePass)
	{
		_eventCalendarModel = calendarModel;
		_battlePass = battlePass;
		_eventBattlePassButtonView.Set(_eventCalendarModel, _battlePass);
		ResetRewardState(_battlePass.ID);
	}

	private void Awake()
	{
		_battlePassMetaWindow = _windowsManager.Get<BattlePassMetaWindow>();
		_buttonClickStream = _button.OnClickAsObservable().Subscribe(delegate
		{
			OpenWindow();
		});
	}

	private void OnDestroy()
	{
		_buttonClickStream?.Dispose();
		_calendarStream?.Dispose();
	}

	private void ResetRewardState(int battlePassId)
	{
		string text = _mapperProvider.GetEventMapper(battlePassId).bp_resource;
		if (string.IsNullOrEmpty(text))
		{
			text = "bp_points";
		}
		SelectorTools.GetResourceEnumValueByConfigKey(text, out var currency);
		if (_currencyProcessor.GetCount(currency) != 0)
		{
			return;
		}
		foreach (RewardWithManyConditions reward in _battlePass.AllRewardContainer.Rewards)
		{
			reward.ResetState();
		}
	}

	private void OpenWindow()
	{
		_battlePassMetaWindow.OpenFromEvent(_eventCalendarModel, _battlePass);
	}
}
