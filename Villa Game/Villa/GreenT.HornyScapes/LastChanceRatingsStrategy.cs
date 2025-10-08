using System;
using System.Linq;
using GreenT.AssetBundles;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Events;
using GreenT.UI;
using Merge.Meta.RoomObjects;
using StripClub.Model.Shop.Data;
using UniRx;

namespace GreenT.HornyScapes;

public sealed class LastChanceRatingsStrategy : BaseLastChanceStrategy, IDisposable
{
	private readonly EventSettingsProvider _eventSettingsProvider;

	private readonly EventBundleDataLoader _eventBundleLoader;

	private readonly EventShopBundleLoader _eventShopBundleLoader;

	private readonly EventMapperProvider _eventMapperProvider;

	private readonly CalendarManager _calendarManager;

	private readonly CalendarQueue _calendarQueue;

	private readonly BundlesProviderBase _bundlesProvider;

	private readonly EventRatingControllerFactory _ratingControllerFactory;

	private readonly RatingManager _ratingManager;

	private readonly RatingDataFactory _ratingDataFactory;

	private readonly RatingDataManager _ratingDataManager;

	private readonly RatingRewardService _ratingRewardService;

	private readonly RatingControllerManager _ratingControllerManager;

	private readonly GameStarter _gameStarter;

	private readonly LastChanceEventBundleProvider _eventBundlesProvider;

	private EventRatingWindowView _eventRatingWindowView;

	private readonly IWindowsManager _windowsManager;

	private readonly CompositeDisposable _initDisposables = new CompositeDisposable();

	private readonly CompositeDisposable _collectDisposables = new CompositeDisposable();

	public override event Action<LastChance> Stopped;

	public LastChanceRatingsStrategy(EventMapperProvider eventMapperProvider, CalendarManager calendarManager, CalendarQueue calendarQueue, EventRatingControllerFactory ratingControllerFactory, RatingManager ratingManager, RatingDataFactory ratingDataFactory, RatingDataManager ratingDataManager, RatingRewardService ratingRewardService, RatingControllerManager ratingControllerManager, LastChanceEventBundleProvider eventBundlesProvider, EventSettingsProvider eventSettingsProvider, EventBundleDataLoader eventBundleLoader, EventShopBundleLoader eventShopBundleLoader, BundlesProviderBase bundlesProvider, IWindowsManager windowsManager, GameStarter gameStarter)
	{
		_eventMapperProvider = eventMapperProvider;
		_calendarManager = calendarManager;
		_calendarQueue = calendarQueue;
		_ratingControllerFactory = ratingControllerFactory;
		_ratingManager = ratingManager;
		_ratingDataFactory = ratingDataFactory;
		_ratingDataManager = ratingDataManager;
		_ratingRewardService = ratingRewardService;
		_ratingControllerManager = ratingControllerManager;
		_eventBundlesProvider = eventBundlesProvider;
		_eventSettingsProvider = eventSettingsProvider;
		_eventBundleLoader = eventBundleLoader;
		_eventShopBundleLoader = eventShopBundleLoader;
		_bundlesProvider = bundlesProvider;
		_windowsManager = windowsManager;
		_gameStarter = gameStarter;
	}

	public void Dispose()
	{
		_initDisposables?.Clear();
		_initDisposables?.Dispose();
		_collectDisposables?.Clear();
		_collectDisposables?.Dispose();
	}

	public override void Init(LastChance lastChance, Action<LastChance> onFinish)
	{
		TryGetEventRatingWindow();
		EventMapper eventMapper = _eventMapperProvider.GetEventMapper(lastChance.EventId);
		CalendarModel calendarModel2 = _calendarManager.Collection.FirstOrDefault((CalendarModel calendarModel) => calendarModel.UniqID == lastChance.CalendarId);
		if (eventMapper.rating_id != 0)
		{
			TryPrepareRating(eventMapper.rating_id, eventMapper.event_id, isGlobal: true, calendarModel2, isCurrencyTrackable: false);
		}
		if (eventMapper.group_rating_id != 0)
		{
			TryPrepareRating(eventMapper.group_rating_id, eventMapper.event_id, isGlobal: false, calendarModel2, isCurrencyTrackable: false);
		}
		if (_eventBundlesProvider.Contains(lastChance.EventId))
		{
			return;
		}
		Event @event = _eventSettingsProvider.GetEvent(lastChance.EventId);
		if (@event != null)
		{
			OnBundleRecieved(eventMapper, lastChance, @event.Bundle, onFinish);
			return;
		}
		(EventBundleData _eventBundleData, IAssetBundle _shopBundle) buffer = (_eventBundleData: null, _shopBundle: null);
		(from _ in _eventBundleLoader.Load(eventMapper.event_bundle).Do(delegate(EventBundleData _eventBundleData)
			{
				buffer._eventBundleData = _eventBundleData;
			}).ContinueWith(_eventShopBundleLoader.Load(eventMapper.event_bundle).Do(delegate(IAssetBundle _shopBundle)
			{
				buffer._shopBundle = _shopBundle;
			}))
			select buffer).Do(delegate((EventBundleData _eventBundleData, IAssetBundle _shopBundle) _bundles)
		{
			_bundlesProvider.TryAdd(ContentSource.EventBundle, _bundles._shopBundle);
			OnBundleRecieved(eventMapper, lastChance, _bundles._eventBundleData, onFinish);
		}).Subscribe().AddTo(_initDisposables);
	}

	public override void Stop(LastChance lastChance)
	{
		_gameStarter.IsGameActive.Where((bool x) => x).Subscribe(delegate
		{
			CollectRewards(lastChance);
		}).AddTo(_collectDisposables);
	}

	public override void Execute(LastChance lastChance)
	{
		TryGetEventRatingWindow();
		EventMapper eventMapper = _eventMapperProvider.GetEventMapper(lastChance.EventId);
		InitRatingViews(eventMapper, lastChance);
		if (eventMapper.rating_id != 0 || eventMapper.group_rating_id != 0)
		{
			_eventRatingWindowView.TryActivateElements(eventMapper.rating_id, eventMapper.group_rating_id);
			_eventRatingWindowView.InitDescription(lastChance.EventId, isOver: true);
			_eventRatingWindowView.InitButtons(eventMapper.rating_id);
			_eventRatingWindowView.InitBackground(_eventBundlesProvider.TryGet(lastChance.EventId).Type);
			_eventRatingWindowView.SetActiveInfo(isActive: false);
		}
		_eventRatingWindowView.Open();
	}

	private void OnBundleRecieved(EventMapper eventMapper, LastChance lastChance, EventBundleData eventBundleData, Action<LastChance> onFinish)
	{
		InitRatingViews(eventMapper, lastChance);
		_eventBundlesProvider.TryAdd(lastChance.EventId, eventBundleData);
		_eventRatingWindowView.ForceAutoUpdate(eventMapper.rating_id, eventMapper.group_rating_id);
		onFinish(lastChance);
	}

	private void InitRatingViews(EventMapper eventMapper, LastChance lastChance)
	{
		if (eventMapper.rating_id != 0)
		{
			_eventRatingWindowView.InitGlobal(lastChance.EventId, lastChance.CalendarId, eventMapper.rating_id);
		}
		if (eventMapper.group_rating_id != 0)
		{
			_eventRatingWindowView.InitGroup(lastChance.EventId, lastChance.CalendarId, eventMapper.group_rating_id);
		}
	}

	private void TryPrepareRating(int ratingId, int eventId, bool isGlobal, CalendarModel calendarModel, bool isCurrencyTrackable, bool withController = true)
	{
		RatingData ratingData = _ratingDataManager.TryGetRatingData(eventId, calendarModel.UniqID, ratingId);
		if (ratingData != null)
		{
			if (withController)
			{
				_ratingControllerManager.TryGetRatingController(ratingData).Initialize();
			}
			return;
		}
		Rating ratingInfo = _ratingManager.GetRatingInfo(ratingId);
		ratingData = _ratingDataFactory.Create(eventId, calendarModel.UniqID, isGlobal, ratingInfo);
		if (withController)
		{
			_ratingControllerFactory.Create(ratingData, isCurrencyTrackable, calendarModel).Initialize();
		}
	}

	private void TryGetEventRatingWindow()
	{
		if (_eventRatingWindowView == null)
		{
			_eventRatingWindowView = _windowsManager.Get<EventRatingWindowView>();
		}
	}

	private void CollectRewards(LastChance lastChance)
	{
		TryGetEventRatingWindow();
		_eventRatingWindowView.Close();
		EventMapper eventMapper = _eventMapperProvider.GetEventMapper(lastChance.EventId);
		CalendarModel calendarModel2 = _calendarManager.Collection.FirstOrDefault((CalendarModel calendarModel) => calendarModel.UniqID == lastChance.CalendarId);
		if (eventMapper.rating_id != 0)
		{
			TryPrepareRating(eventMapper.rating_id, eventMapper.event_id, isGlobal: true, calendarModel2, isCurrencyTrackable: false, withController: false);
			_ratingRewardService.TryAutoClaimReward(lastChance.EventId, lastChance.CalendarId, eventMapper.rating_id);
		}
		if (eventMapper.group_rating_id != 0)
		{
			TryPrepareRating(eventMapper.group_rating_id, eventMapper.event_id, isGlobal: false, calendarModel2, isCurrencyTrackable: false, withController: false);
			_ratingRewardService.TryAutoClaimReward(lastChance.EventId, lastChance.CalendarId, eventMapper.group_rating_id);
		}
		if (!calendarModel2.WasEnded && calendarModel2.CalendarState.Value == EntityStatus.Rewarded)
		{
			calendarModel2.CalendarStrategy.Clean(calendarModel2);
			calendarModel2.WasEnded = true;
			if (_calendarQueue.IsCalendarActive(calendarModel2))
			{
				_calendarQueue.Remove(calendarModel2);
			}
		}
		_collectDisposables?.Clear();
		_collectDisposables?.Dispose();
	}
}
