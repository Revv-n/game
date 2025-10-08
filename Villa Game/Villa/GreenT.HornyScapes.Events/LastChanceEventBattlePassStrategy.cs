using System;
using System.Linq;
using GreenT.AssetBundles;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.BattlePassSpace.UI.MetaWindow;
using GreenT.HornyScapes.Characters;
using GreenT.HornyScapes.Lootboxes;
using GreenT.Settings.Data;
using GreenT.Types;
using GreenT.UI;
using Merge.Meta.RoomObjects;
using StripClub.Model;
using UniRx;

namespace GreenT.HornyScapes.Events;

public sealed class LastChanceEventBattlePassStrategy : BaseLastChanceStrategy, IDisposable
{
	private BattlePassMetaWindow _battlePassMetaWindow;

	private readonly EventMapperProvider _eventMapperProvider;

	private readonly LastChanceEventBundleProvider _eventBundlesProvider;

	private readonly EventSettingsProvider _eventSettingsProvider;

	private readonly CharacterProvider _characterProvider;

	private readonly IWindowsManager _windowsManager;

	private readonly EventBattlePassChecker _battlePassChecker;

	private readonly BattlePassSettingsProvider _battlePassSettingsProvider;

	private readonly BattlePassMapperProvider _battlePassMapperProvider;

	private readonly CalendarQueue _calendarQueue;

	private readonly BundleLoader _bundleLoader;

	private readonly CalendarManager _calendarManager;

	private readonly EventBattlePassDataCleaner _dataCleaner;

	private readonly GameStarter _gameStarter;

	private readonly GameSettings _gameSettings;

	private readonly CompositeDisposable _initDisposables = new CompositeDisposable();

	private readonly CompositeDisposable _stopDisposables = new CompositeDisposable();

	public override event Action<LastChance> Stopped;

	public LastChanceEventBattlePassStrategy(EventMapperProvider eventMapperProvider, LastChanceEventBundleProvider eventBundlesProvider, EventSettingsProvider eventSettingsProvider, EventBundleDataLoader eventBundleLoader, EventShopBundleLoader eventShopBundleLoader, BundlesProviderBase bundlesProvider, IWindowsManager windowsManager, EventBattlePassChecker battlePassChecker, BattlePassSettingsProvider battlePassSettingsProvider, BattlePassMapperProvider battlePassMapperProvider, CalendarQueue calendarQueue, CalendarManager calendarManager, EventBattlePassDataCleaner dataCleaner, GameStarter gameStarter, GameSettings gameSettings, CharacterProvider characterProvider, BundleLoader bundleLoader)
	{
		_eventMapperProvider = eventMapperProvider;
		_eventBundlesProvider = eventBundlesProvider;
		_eventSettingsProvider = eventSettingsProvider;
		_windowsManager = windowsManager;
		_battlePassChecker = battlePassChecker;
		_battlePassSettingsProvider = battlePassSettingsProvider;
		_battlePassMapperProvider = battlePassMapperProvider;
		_calendarQueue = calendarQueue;
		_calendarManager = calendarManager;
		_dataCleaner = dataCleaner;
		_gameStarter = gameStarter;
		_gameSettings = gameSettings;
		_characterProvider = characterProvider;
		_bundleLoader = bundleLoader;
	}

	public override void Init(LastChance lastChance, Action<LastChance> onFinish)
	{
		EventMapper eventMapper = _eventMapperProvider.GetEventMapper(lastChance.EventId);
		int battlePassId = eventMapper.bp_id;
		if (!_eventBundlesProvider.Contains(lastChance.EventId))
		{
			Event @event = _eventSettingsProvider.GetEvent(lastChance.EventId);
			if (@event == null)
			{
				_bundleLoader.Load<EventBundleData>(BundleType.EventShop, eventMapper.event_bundle).SelectMany((EventBundleData bundle) => _battlePassChecker.LoadBattlePass(battlePassId).Do(delegate
				{
					OnBundleReceived(lastChance, bundle, onFinish);
				})).Subscribe()
					.AddTo(_initDisposables);
			}
			else
			{
				OnBundleReceived(lastChance, @event.Bundle, onFinish);
			}
		}
		_calendarQueue.OnCalendarActive().Subscribe(delegate(CalendarModel model)
		{
			CheckCalendar(model, lastChance);
		}).AddTo(_initDisposables);
	}

	public override void Stop(LastChance lastChance)
	{
		EventMapper eventMapper = _eventMapperProvider.GetEventMapper(lastChance.EventId);
		int battlePassId = eventMapper.bp_id;
		if (!_battlePassSettingsProvider.TryGetBattlePass(battlePassId, out var battlePass2))
		{
			_battlePassChecker.LoadBattlePass(battlePassId).Subscribe(delegate(BattlePass battlePass)
			{
				CheckBattlePass(battlePassId, battlePass, lastChance);
			}).AddTo(_stopDisposables);
		}
		else
		{
			_ = lastChance.CalendarId;
			if (_calendarManager.Collection.FirstOrDefault((CalendarModel calendarModel) => calendarModel.UniqID == lastChance.CalendarId).CalendarState.Value != EntityStatus.Rewarded)
			{
				CollectRewards(battlePass2);
			}
			ClearBattlePass(battlePass2);
			ClearCalendar(lastChance);
		}
		Stopped?.Invoke(lastChance);
	}

	public override void Execute(LastChance lastChance)
	{
		TryGetBattlePassMetaWindow();
		int bp_id = _eventMapperProvider.GetEventMapper(lastChance.EventId).bp_id;
		CalendarModel calendarModel2 = _calendarManager.Collection.FirstOrDefault((CalendarModel calendarModel) => calendarModel.UniqID == lastChance.CalendarId);
		if (bp_id != 0 && calendarModel2 != null && _battlePassSettingsProvider.TryGetBattlePass(bp_id, out var battlePass))
		{
			_battlePassMetaWindow.OpenFromEvent(calendarModel2, battlePass);
		}
	}

	public void Dispose()
	{
		_initDisposables?.Clear();
		_initDisposables?.Dispose();
		_stopDisposables?.Clear();
		_stopDisposables?.Dispose();
	}

	private void OnBundleReceived(LastChance lastChance, EventBundleData eventBundleData, Action<LastChance> onFinish)
	{
		_eventBundlesProvider.TryAdd(lastChance.EventId, eventBundleData);
		UpdateCurrency(eventBundleData, lastChance);
		onFinish(lastChance);
	}

	private void UpdateCurrency(EventBundleData eventBundleData, LastChance lastChance)
	{
		int bp_id = _eventMapperProvider.GetEventMapper(lastChance.EventId).bp_id;
		string text = _battlePassMapperProvider.GetEventMapper(bp_id).bp_resource;
		if (string.IsNullOrEmpty(text))
		{
			text = "bp_points";
		}
		SelectorTools.GetResourceEnumValueByConfigKey(text, out var currency);
		bool flag = currency == CurrencyType.EventXP;
		CurrencySettings currencySettings = _gameSettings.CurrencySettings[currency, default(CompositeIdentificator)];
		currencySettings.SetSprite(flag ? eventBundleData.Target : eventBundleData.Currency);
		currencySettings.SetAlternativeSprite(flag ? eventBundleData.Target : eventBundleData.AlternativeCurrency);
		currencySettings.SetLocalization(flag ? eventBundleData.TargetKeyLoc : eventBundleData.CurrencyKeyLoc);
	}

	private void CheckCalendar(CalendarModel calendarModel, LastChance lastChance)
	{
		if (calendarModel.UniqID != lastChance.CalendarId && calendarModel.EventType == EventStructureType.Event)
		{
			Stop(lastChance);
		}
	}

	private void TryGetBattlePassMetaWindow()
	{
		if (_battlePassMetaWindow == null)
		{
			_battlePassMetaWindow = _windowsManager.Get<BattlePassMetaWindow>();
		}
	}

	private void CheckBattlePass(int battlePassId, BattlePass battlePass, LastChance lastChance)
	{
		if (battlePass.ID == battlePassId)
		{
			_gameStarter.IsGameActive.Where((bool x) => x).Subscribe(delegate
			{
				CollectRewards(battlePass);
				ClearBattlePass(battlePass);
				ClearCalendar(lastChance);
			}).AddTo(_stopDisposables);
		}
	}

	private void ClearBattlePass(BattlePass battlePass)
	{
		_dataCleaner.CleanData(battlePass);
		_battlePassSettingsProvider.RemoveBattlePass(battlePass);
		_stopDisposables?.Clear();
		_stopDisposables?.Dispose();
	}

	private void CollectRewards(BattlePass battlePass)
	{
		foreach (RewardWithManyConditions item in battlePass.AllRewardContainer.Rewards.Where((RewardWithManyConditions reward) => reward.IsComplete))
		{
			item.TryCollectReward();
		}
	}

	private void ClearCalendar(LastChance lastChance)
	{
		_eventMapperProvider.GetEventMapper(lastChance.EventId);
		CalendarModel calendarModel2 = _calendarManager.Collection.FirstOrDefault((CalendarModel calendarModel) => calendarModel.UniqID == lastChance.CalendarId);
		if (!calendarModel2.WasEnded && calendarModel2.CalendarState.Value == EntityStatus.Rewarded)
		{
			calendarModel2.CalendarStrategy.Clean(calendarModel2);
			calendarModel2.WasEnded = true;
			if (_calendarQueue.IsCalendarActive(calendarModel2))
			{
				_calendarQueue.Remove(calendarModel2);
			}
		}
	}
}
