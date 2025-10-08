using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.AssetBundles;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Characters;
using GreenT.HornyScapes.Characters.Skins;
using GreenT.HornyScapes.Characters.Skins.Content;
using GreenT.HornyScapes.Stories;
using GreenT.Settings.Data;
using StripClub.Model;
using StripClub.Model.Character;
using StripClub.Model.Shop.Data;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Events;

public class EventCalendarLoader : ICalendarLoader
{
	private readonly EventFactory _eventFactory;

	private readonly EventMapperProvider _eventMapperProvider;

	private readonly EventMergeIconsLoader _iconsLoader;

	private readonly EventMergeIconProvider _iconProvider;

	private readonly CalendarStrategyFactory _calendarStrategyFactory;

	private readonly SkinManager _skinManager;

	private readonly BundleLoader _bundleLoader;

	private readonly CharacterProvider _characterProvider;

	private readonly SkinDataLoadingController _skinLoader;

	private readonly EventSettingsProvider _eventSettingsProvider;

	private readonly EventBattlePassChecker _battlePassChecker;

	private readonly CompositeDisposable _calendarLoadDisposable = new CompositeDisposable();

	private readonly StoryCluster _storyManagerCluster;

	private readonly Dictionary<CalendarModel, IDisposable> _stateMap = new Dictionary<CalendarModel, IDisposable>();

	private readonly Subject<(CalendarModel, CalendarLoadingStatus)> _onLoadingStateChangeSubject = new Subject<(CalendarModel, CalendarLoadingStatus)>();

	public EventCalendarLoader(EventFactory eventFactory, BundleLoader bundleLoader, EventMapperProvider eventMapperProvider, EventMergeIconsLoader iconsLoader, EventMergeIconProvider iconProvider, CalendarStrategyFactory calendarStrategyFactory, CharacterProvider characterProvider, EventSettingsProvider eventSettingsProvider, SkinDataLoadingController skinLoader, StoryCluster storyManagerCluster, EventBattlePassChecker battlePassChecker)
	{
		_iconsLoader = iconsLoader;
		_iconProvider = iconProvider;
		_eventFactory = eventFactory;
		_eventMapperProvider = eventMapperProvider;
		_calendarStrategyFactory = calendarStrategyFactory;
		_characterProvider = characterProvider;
		_eventSettingsProvider = eventSettingsProvider;
		_skinLoader = skinLoader;
		_storyManagerCluster = storyManagerCluster;
		_battlePassChecker = battlePassChecker;
		_bundleLoader = bundleLoader;
	}

	public void SetLoadingStream(CalendarModel calendarModel)
	{
		IDisposable value = calendarModel.LoadingStatus.Subscribe(delegate(CalendarLoadingStatus state)
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
		return from tuple in _onLoadingStateChangeSubject
			where tuple.Item1.EventType == type && tuple.Item1.LoadingStatus.Value == loadingStatus
			select tuple.Item1;
	}

	public IObservable<CalendarModel> Load(CalendarModel calendarModel)
	{
		IEventMapper eventMapper2 = calendarModel.EventMapper;
		EventMapper eventMapper = eventMapper2 as EventMapper;
		if (eventMapper == null)
		{
			return Observable.Empty<CalendarModel>();
		}
		SetLoadingStream(calendarModel);
		calendarModel.SetLoadingLifeStatus(CalendarLoadingStatus.Start);
		calendarModel.SetLoadingStatus(CalendarModelLoadingState.InProgress);
		IConnectableObservable<ICharacter> connectableObservable = _characterProvider.Get(eventMapper.characters).Publish();
		IConnectableObservable<CharacterStories> connectableObservable2 = _characterProvider.GetStory(eventMapper.characters.Where(_storyManagerCluster.IsPhraseEnable).ToArray()).Publish();
		IConnectableObservable<Event> connectableObservable3 = (from data in _bundleLoader.LoadBundle(BundleType.EventShop, eventMapper.event_bundle.ToLower(), ContentSource.EventBundle).SelectMany((Unit _) => _bundleLoader.Load<EventBundleData>(BundleType.Events, eventMapper.event_bundle.ToLower(), ContentSource.EventBundle))
			select _eventFactory.Create(eventMapper, data, calendarModel)).Do(delegate(IBundleProvider<EventBundleData> _event)
		{
			_eventSettingsProvider.Add(_event as Event);
		}).SelectMany((IBundleProvider<EventBundleData> _event) => SelectGirlsId(_event as Event)).Do(delegate
		{
		})
			.SelectMany((Event _event) => _eventFactory.TryRegisterRating(_event))
			.Catch(delegate(Exception ex)
			{
				calendarModel.SetLoadingStatus(CalendarModelLoadingState.Failed);
				Debug.LogError(GetType().Name + ": Error when creating " + calendarModel.EventMapper.Bundle + " " + $"CalendarId: {calendarModel.UniqID} " + $"EventId: {calendarModel.EventMapper.ID} " + $"with structure: {EventStructureType.Event}\n{ex}");
				throw ex;
			})
			.Publish();
		connectableObservable3.Subscribe(delegate(Event provider)
		{
			try
			{
				ICalendarStateStrategy calendarStateStrategy = _calendarStrategyFactory.Create(calendarModel.EventType, calendarModel);
				calendarModel.Set(provider, calendarStateStrategy, eventMapper.event_id);
				calendarModel.SetLoadingStatus(CalendarModelLoadingState.Success);
				calendarModel.SetLoadingLifeStatus(CalendarLoadingStatus.End);
				Remove(calendarModel);
			}
			catch (Exception innerException)
			{
				calendarModel.SetLoadingStatus(CalendarModelLoadingState.Failed);
				throw innerException.SendException(GetType().Name + ": Error when creating Event" + $" with structure: {EventStructureType.Event}" + "\n");
			}
		});
		IConnectableObservable<IEnumerable<Sprite>> connectableObservable4 = _iconsLoader.Load(eventMapper.Bundle).Do(delegate(IEnumerable<Sprite> sprites)
		{
			_iconProvider.AddRange(sprites);
		}).Publish();
		int bp_id = eventMapper.bp_id;
		IObservable<BattlePass> source = _battlePassChecker.LoadBattlePass(bp_id).Debug($"EventBattlePassLoader loaded, battlePass id = {bp_id}");
		IConnectableObservable<CalendarModel> connectableObservable5 = (from _ in connectableObservable3.AsUnitObservable().Merge(connectableObservable.AsUnitObservable()).Merge(connectableObservable2.AsUnitObservable())
				.Merge(connectableObservable4.AsUnitObservable())
				.Merge(source.AsUnitObservable())
				.LastOrDefault()
				.Debug($"Event loaded, battlePass id = {eventMapper.bp_id}")
			select calendarModel).Publish();
		connectableObservable.Connect().AddTo(_calendarLoadDisposable);
		connectableObservable2.Connect().AddTo(_calendarLoadDisposable);
		connectableObservable3.Connect().AddTo(_calendarLoadDisposable);
		connectableObservable4.Connect().AddTo(_calendarLoadDisposable);
		connectableObservable5.Connect().AddTo(_calendarLoadDisposable);
		return connectableObservable5;
	}

	private IObservable<Event> SelectGirlsId(Event @event)
	{
		List<LinkedContent> source = (from linkedContent in @event.GetAllRewardsContent()
			select (linkedContent)).ToList();
		IObservable<Unit> first = (from girl in source.OfType<CardLinkedContent>()
			select girl.Card.ID into id
			select TryLoadCharacter(id, @event) into x
			select from character in x
				where _storyManagerCluster.IsPhraseEnable(character.ID)
				select character into y
				select _characterProvider.GetStory(y.ID)).Concat().DefaultIfEmpty().LastOrDefault()
			.AsUnitObservable();
		IObservable<Unit> observable = (from girl in source.OfType<SkinLinkedContent>()
			select girl.Skin.ID into id
			select TryLoadSkin(id, @event)).Concat().DefaultIfEmpty().LastOrDefault()
			.AsUnitObservable();
		return from _ in first.Merge(observable).LastOrDefault().Debug($"Finished character and skin data loading for event {@event.EventId}", LogType.Events)
			select @event;
	}

	private IObservable<ICharacter> TryLoadCharacter(int id, Event @event)
	{
		try
		{
			return _characterProvider.Get(id);
		}
		catch (Exception innerException)
		{
			throw innerException.SendException($"Can't load character {id} for event {@event.EventId}");
		}
	}

	private IObservable<SkinData> TryLoadSkin(int id, Event @event)
	{
		try
		{
			return _skinLoader.InsertDataOnLoad(id);
		}
		catch (Exception innerException)
		{
			throw innerException.SendException($"Can't load skin {id} for event {@event.EventId}");
		}
	}

	public IEventMapper GetEventMapper(int event_id)
	{
		return _eventMapperProvider.GetEventMapper(event_id);
	}
}
