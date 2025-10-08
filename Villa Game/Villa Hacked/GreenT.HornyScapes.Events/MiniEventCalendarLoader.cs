using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.AssetBundles;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Characters.Skins;
using GreenT.HornyScapes.Characters.Skins.Content;
using GreenT.HornyScapes.MiniEvents;
using GreenT.Types;
using StripClub.Model.Shop.Data;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Events;

public sealed class MiniEventCalendarLoader : ICalendarLoader
{
	private readonly MiniEventFactory _miniEventFactory;

	private readonly MiniEventMapperManager _miniEventMapperManager;

	private readonly ActivitiesManager _activitiesManager;

	private readonly ActivitiesShopManager _activitiesShopManager;

	private readonly ActivitiesQuestManager _activitiesQuestManager;

	private readonly CalendarStrategyFactory _calendarStrategyFactory;

	private readonly MiniEventSettingsProvider _miniEventSettingsProvider;

	private readonly MiniEventBundleDataLoader _miniEventBundleLoader;

	private readonly MiniEventShopBundleDataLoader _miniEventShopBundleDataLoader;

	private readonly MiniEventsBundlesProvider _miniEventsBundlesProvider;

	private readonly BundlesProviderBase _bundlesProvider;

	private readonly MiniEventMergeIconsLoader _mergeIconsLoader;

	private readonly MiniEventMergeIconsProvider _mergeIconsProvider;

	private readonly CompositeDisposable _calendarLoadDisposable = new CompositeDisposable();

	private readonly MiniEventTaskManager _miniEventTaskManager;

	private readonly SkinDataLoadingController _skinDataLoadingController;

	private readonly Dictionary<CalendarModel, IDisposable> _stateMap = new Dictionary<CalendarModel, IDisposable>();

	private readonly Subject<(CalendarModel, CalendarLoadingStatus)> _onLoadingStateChangeSubject = new Subject<(CalendarModel, CalendarLoadingStatus)>();

	public MiniEventCalendarLoader(MiniEventFactory miniEventFactory, ActivitiesManager activitiesManager, ActivitiesShopManager activitiesShopManager, ActivitiesQuestManager activitiesQuestManager, MiniEventMapperManager miniEventMapperManager, MiniEventBundleDataLoader miniEventBundleLoader, MiniEventShopBundleDataLoader miniEventShopBundleDataLoader, CalendarStrategyFactory calendarStrategyFactory, MiniEventSettingsProvider miniEventSettingsProvider, MiniEventsBundlesProvider miniEventBundlesProvider, BundlesProviderBase bundlesProvider, MiniEventMergeIconsLoader mergeIconsLoader, MiniEventMergeIconsProvider mergeIconsProvider, MiniEventTaskManager miniEventTaskManager, SkinDataLoadingController skinDataLoadingController)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		_miniEventFactory = miniEventFactory;
		_activitiesManager = activitiesManager;
		_activitiesShopManager = activitiesShopManager;
		_activitiesQuestManager = activitiesQuestManager;
		_miniEventMapperManager = miniEventMapperManager;
		_miniEventBundleLoader = miniEventBundleLoader;
		_miniEventShopBundleDataLoader = miniEventShopBundleDataLoader;
		_calendarStrategyFactory = calendarStrategyFactory;
		_miniEventSettingsProvider = miniEventSettingsProvider;
		_miniEventsBundlesProvider = miniEventBundlesProvider;
		_bundlesProvider = bundlesProvider;
		_mergeIconsProvider = mergeIconsProvider;
		_mergeIconsLoader = mergeIconsLoader;
		_miniEventTaskManager = miniEventTaskManager;
		_skinDataLoadingController = skinDataLoadingController;
	}

	public void SetLoadingStream(CalendarModel calendarModel)
	{
		IDisposable value = ObservableExtensions.Subscribe<CalendarLoadingStatus>((IObservable<CalendarLoadingStatus>)calendarModel.LoadingStatus, (Action<CalendarLoadingStatus>)delegate(CalendarLoadingStatus state)
		{
			_onLoadingStateChangeSubject.OnNext((calendarModel, state));
		});
		_stateMap.Add(calendarModel, value);
	}

	private void Remove(CalendarModel calendarModel)
	{
		_stateMap[calendarModel]?.Dispose();
		_stateMap.Remove(calendarModel);
	}

	public IObservable<CalendarModel> OnConcreteCalendarLoadingStateChange(EventStructureType type, CalendarLoadingStatus loadingStatus)
	{
		return Observable.Select<(CalendarModel, CalendarLoadingStatus), CalendarModel>(Observable.Where<(CalendarModel, CalendarLoadingStatus)>((IObservable<(CalendarModel, CalendarLoadingStatus)>)_onLoadingStateChangeSubject, (Func<(CalendarModel, CalendarLoadingStatus), bool>)(((CalendarModel, CalendarLoadingStatus) tuple) => tuple.Item1.EventType == type && tuple.Item1.LoadingStatus.Value == loadingStatus)), (Func<(CalendarModel, CalendarLoadingStatus), CalendarModel>)(((CalendarModel, CalendarLoadingStatus) tuple) => tuple.Item1));
	}

	public IObservable<CalendarModel> Load(CalendarModel calendarModel)
	{
		IEventMapper eventMapper = calendarModel.EventMapper;
		MiniEventMapper miniEventMapper = eventMapper as MiniEventMapper;
		if (miniEventMapper == null)
		{
			return Observable.Empty<CalendarModel>();
		}
		SetLoadingStream(calendarModel);
		calendarModel.SetLoadingStatus(CalendarModelLoadingState.InProgress);
		calendarModel.SetLoadingLifeStatus(CalendarLoadingStatus.Start);
		ActivityMapper activityInfo = _activitiesManager.GetActivityInfo(miniEventMapper.activity_id);
		activityInfo.quest_tabs.Any();
		activityInfo.shop_tabs.Any();
		int[] quest_tabs = activityInfo.quest_tabs;
		int[] shop_tabs = activityInfo.shop_tabs;
		SetupTabsKeys(miniEventMapper.id, activityInfo.quest_tabs, quest_tabs.Any(), TabType.Task);
		SetupTabsKeys(miniEventMapper.id, activityInfo.shop_tabs, shop_tabs.Any(), TabType.Shop);
		string bundle = miniEventMapper.Bundle;
		int iD = miniEventMapper.ID;
		IObservable<MiniEvent> observable = Observable.Catch<MiniEvent, Exception>(Observable.Do<MiniEvent>(Observable.Select<IEnumerable<Sprite>, MiniEvent>(Observable.Do<IEnumerable<Sprite>>(Observable.ContinueWith<SkinData, IEnumerable<Sprite>>(Observable.ContinueWith<IAssetBundle, SkinData>(Observable.ContinueWith<IAssetBundle, IAssetBundle>(AddLoadShopBundle(bundle), AddLoadBaseBundle(bundle, iD)), AddLoadSkinData(activityInfo.quest_tabs, activityInfo.shop_tabs)), _mergeIconsLoader.Load(bundle)), (Action<IEnumerable<Sprite>>)_mergeIconsProvider.AddRange), (Func<IEnumerable<Sprite>, MiniEvent>)((IEnumerable<Sprite> data) => CreateMiniEvent(calendarModel, miniEventMapper))), (Action<MiniEvent>)delegate(MiniEvent miniEvent)
		{
			_miniEventSettingsProvider.Add(miniEvent);
			calendarModel.SetLoadingLifeStatus(CalendarLoadingStatus.End);
			Remove(calendarModel);
		}), (Func<Exception, IObservable<MiniEvent>>)delegate(Exception ex)
		{
			throw ex.SendException(string.Format("{0}: Error when creating {1} with structure: {2}\n", GetType().Name, "MiniEvent", EventStructureType.Mini));
		});
		Observable.Do<CalendarModel>(Observable.Select<MiniEvent, CalendarModel>(Observable.Catch<MiniEvent, Exception>(Observable.Do<MiniEvent>(Observable.Select<IEnumerable<Sprite>, MiniEvent>(Observable.Do<IEnumerable<Sprite>>(Observable.ContinueWith<IAssetBundle, IEnumerable<Sprite>>(Observable.ContinueWith<IAssetBundle, IAssetBundle>(AddLoadShopBundle(miniEventMapper.Bundle), AddLoadBaseBundle(miniEventMapper.Bundle, miniEventMapper.id)), (Func<IAssetBundle, IObservable<IEnumerable<Sprite>>>)((IAssetBundle _) => _mergeIconsLoader.Load(miniEventMapper.bundle))), (Action<IEnumerable<Sprite>>)delegate(IEnumerable<Sprite> icons)
		{
			_mergeIconsProvider.AddRange(icons);
		}), (Func<IEnumerable<Sprite>, MiniEvent>)((IEnumerable<Sprite> data) => CreateMiniEvent(calendarModel, miniEventMapper))), (Action<MiniEvent>)delegate(MiniEvent miniEvent)
		{
			_miniEventSettingsProvider.Add(miniEvent);
		}), (Func<Exception, IObservable<MiniEvent>>)delegate(Exception ex)
		{
			calendarModel.SetLoadingStatus(CalendarModelLoadingState.Failed);
			throw ex.SendException(GetType().Name + ": Error when creating MiniEvent" + $" with structure: {EventStructureType.Mini}" + "\n");
		}), (Func<MiniEvent, CalendarModel>)((MiniEvent _) => calendarModel)), (Action<CalendarModel>)delegate
		{
		});
		return Observable.Do<CalendarModel>(Observable.Select<MiniEvent, CalendarModel>(observable, (Func<MiniEvent, CalendarModel>)((MiniEvent _) => calendarModel)), (Action<CalendarModel>)delegate
		{
		});
	}

	public IEventMapper GetEventMapper(int event_id)
	{
		return _miniEventMapperManager.GetMiniEventInfo(event_id);
	}

	private IObservable<IAssetBundle> AddLoadBaseBundle(string key, int miniEventId)
	{
		return Observable.Do<IAssetBundle>(_miniEventBundleLoader.Load(key), (Action<IAssetBundle>)delegate(IAssetBundle bundle)
		{
			_miniEventsBundlesProvider.TryAdd(miniEventId, bundle);
		});
	}

	private IObservable<IAssetBundle> AddLoadShopBundle(string key)
	{
		return Observable.Do<IAssetBundle>(_miniEventShopBundleDataLoader.Load(key), (Action<IAssetBundle>)delegate(IAssetBundle bundle)
		{
			_bundlesProvider.TryAdd(ContentSource.MiniEvent, bundle);
		});
	}

	private IObservable<SkinData> AddLoadSkinData(int[] questTabs, int[] shopTabs)
	{
		IEnumerable<IObservable<SkinData>> first = questTabs.Select((int id) => AddLoadSkinData(id, TabType.Task));
		IEnumerable<IObservable<SkinData>> second = shopTabs.Select((int id) => AddLoadSkinData(id, TabType.Shop));
		IEnumerable<IObservable<SkinData>> enumerable = first.Concat(second);
		if (!enumerable.Any())
		{
			return Observable.Empty<SkinData>();
		}
		return Observable.Merge<SkinData>(enumerable);
	}

	private IObservable<SkinData> AddLoadSkinData(int tabId, TabType tabType)
	{
		int num = tabType switch
		{
			TabType.Task => _activitiesQuestManager.GetActivityInfo(tabId).quest_massive_id, 
			TabType.Shop => _activitiesShopManager.GetActivityInfo(tabId).bank_tab_id, 
			_ => -1, 
		};
		if (num == -1)
		{
			return Observable.Empty<SkinData>();
		}
		CompositeIdentificator taskId = new CompositeIdentificator(num);
		IEnumerable<IObservable<SkinData>> enumerable = from task in _miniEventTaskManager.Tasks
			where task.Identificator == taskId
			where task.Reward is SkinLinkedContent
			select _skinDataLoadingController.InsertDataOnLoad((task.Reward as SkinLinkedContent).Skin);
		if (!enumerable.Any())
		{
			return Observable.Return<SkinData>((SkinData)null);
		}
		return Observable.Merge<SkinData>(enumerable);
	}

	private void SetupTabsKeys(int minieventId, int[] ids, bool isNeed, TabType tabType)
	{
		if (isNeed)
		{
			for (int i = 0; i < ids.Length; i++)
			{
				_miniEventsBundlesProvider.TryAdd(minieventId, ids[i], tabType);
			}
		}
	}

	private MiniEvent CreateMiniEvent(CalendarModel calendarModel, MiniEventMapper miniEventMapper)
	{
		try
		{
			MiniEvent miniEvent = _miniEventFactory.Create(miniEventMapper, calendarModel);
			ICalendarStateStrategy calendarStateStrategy = _calendarStrategyFactory.Create(calendarModel.EventType, calendarModel);
			calendarModel.Set(miniEvent, calendarStateStrategy, miniEvent.EventId);
			calendarModel.SetLoadingStatus(CalendarModelLoadingState.Success);
			return miniEvent;
		}
		catch (Exception innerException)
		{
			calendarModel.SetLoadingStatus(CalendarModelLoadingState.Failed);
			throw innerException.SendException(GetType().Name + ": Error when creating MiniEvents" + $" with structure: {EventStructureType.Mini}" + "\n");
		}
	}
}
