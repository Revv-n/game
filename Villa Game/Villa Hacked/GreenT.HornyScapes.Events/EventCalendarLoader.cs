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
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
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
		IEventMapper eventMapper2 = calendarModel.EventMapper;
		EventMapper eventMapper = eventMapper2 as EventMapper;
		if (eventMapper == null)
		{
			return Observable.Empty<CalendarModel>();
		}
		SetLoadingStream(calendarModel);
		calendarModel.SetLoadingLifeStatus(CalendarLoadingStatus.Start);
		calendarModel.SetLoadingStatus(CalendarModelLoadingState.InProgress);
		IConnectableObservable<ICharacter> val = Observable.Publish<ICharacter>(_characterProvider.Get(eventMapper.characters));
		IConnectableObservable<CharacterStories> val2 = Observable.Publish<CharacterStories>(_characterProvider.GetStory(eventMapper.characters.Where(_storyManagerCluster.IsPhraseEnable).ToArray()));
		IConnectableObservable<Event> val3 = Observable.Publish<Event>(Observable.Catch<Event, Exception>(Observable.SelectMany<Event, Event>(Observable.Do<Event>(Observable.SelectMany<IBundleProvider<EventBundleData>, Event>(Observable.Do<IBundleProvider<EventBundleData>>(Observable.Select<EventBundleData, IBundleProvider<EventBundleData>>(Observable.SelectMany<Unit, EventBundleData>(_bundleLoader.LoadBundle(BundleType.EventShop, eventMapper.event_bundle.ToLower(), ContentSource.EventBundle), (Func<Unit, IObservable<EventBundleData>>)((Unit _) => _bundleLoader.Load<EventBundleData>(BundleType.Events, eventMapper.event_bundle.ToLower(), ContentSource.EventBundle))), (Func<EventBundleData, IBundleProvider<EventBundleData>>)((EventBundleData data) => _eventFactory.Create(eventMapper, data, calendarModel))), (Action<IBundleProvider<EventBundleData>>)delegate(IBundleProvider<EventBundleData> _event)
		{
			_eventSettingsProvider.Add(_event as Event);
		}), (Func<IBundleProvider<EventBundleData>, IObservable<Event>>)((IBundleProvider<EventBundleData> _event) => SelectGirlsId(_event as Event))), (Action<Event>)delegate
		{
		}), (Func<Event, IObservable<Event>>)((Event _event) => _eventFactory.TryRegisterRating(_event))), (Func<Exception, IObservable<Event>>)delegate(Exception ex)
		{
			calendarModel.SetLoadingStatus(CalendarModelLoadingState.Failed);
			Debug.LogError(GetType().Name + ": Error when creating " + calendarModel.EventMapper.Bundle + " " + $"CalendarId: {calendarModel.UniqID} " + $"EventId: {calendarModel.EventMapper.ID} " + $"with structure: {EventStructureType.Event}\n{ex}");
			throw ex;
		}));
		ObservableExtensions.Subscribe<Event>((IObservable<Event>)val3, (Action<Event>)delegate(Event provider)
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
		IConnectableObservable<IEnumerable<Sprite>> val4 = Observable.Publish<IEnumerable<Sprite>>(Observable.Do<IEnumerable<Sprite>>(_iconsLoader.Load(eventMapper.Bundle), (Action<IEnumerable<Sprite>>)delegate(IEnumerable<Sprite> sprites)
		{
			_iconProvider.AddRange(sprites);
		}));
		int bp_id = eventMapper.bp_id;
		IObservable<BattlePass> observable = _battlePassChecker.LoadBattlePass(bp_id).Debug($"EventBattlePassLoader loaded, battlePass id = {bp_id}");
		IConnectableObservable<CalendarModel> obj = Observable.Publish<CalendarModel>(Observable.Select<Unit, CalendarModel>(Observable.LastOrDefault<Unit>(Observable.Merge<Unit>(Observable.Merge<Unit>(Observable.Merge<Unit>(Observable.Merge<Unit>(Observable.AsUnitObservable<Event>((IObservable<Event>)val3), new IObservable<Unit>[1] { Observable.AsUnitObservable<ICharacter>((IObservable<ICharacter>)val) }), new IObservable<Unit>[1] { Observable.AsUnitObservable<CharacterStories>((IObservable<CharacterStories>)val2) }), new IObservable<Unit>[1] { Observable.AsUnitObservable<IEnumerable<Sprite>>((IObservable<IEnumerable<Sprite>>)val4) }), new IObservable<Unit>[1] { Observable.AsUnitObservable<BattlePass>(observable) })).Debug($"Event loaded, battlePass id = {eventMapper.bp_id}"), (Func<Unit, CalendarModel>)((Unit _) => calendarModel)));
		DisposableExtensions.AddTo<IDisposable>(val.Connect(), (ICollection<IDisposable>)_calendarLoadDisposable);
		DisposableExtensions.AddTo<IDisposable>(val2.Connect(), (ICollection<IDisposable>)_calendarLoadDisposable);
		DisposableExtensions.AddTo<IDisposable>(val3.Connect(), (ICollection<IDisposable>)_calendarLoadDisposable);
		DisposableExtensions.AddTo<IDisposable>(val4.Connect(), (ICollection<IDisposable>)_calendarLoadDisposable);
		DisposableExtensions.AddTo<IDisposable>(obj.Connect(), (ICollection<IDisposable>)_calendarLoadDisposable);
		return (IObservable<CalendarModel>)obj;
	}

	private IObservable<Event> SelectGirlsId(Event @event)
	{
		List<LinkedContent> source = (from linkedContent in @event.GetAllRewardsContent()
			select (linkedContent)).ToList();
		IObservable<Unit> observable = Observable.AsUnitObservable<IObservable<CharacterStories>>(Observable.LastOrDefault<IObservable<CharacterStories>>(Observable.DefaultIfEmpty<IObservable<CharacterStories>>(Observable.Concat<IObservable<CharacterStories>>(from girl in source.OfType<CardLinkedContent>()
			select girl.Card.ID into id
			select TryLoadCharacter(id, @event) into x
			select Observable.Select<ICharacter, IObservable<CharacterStories>>(Observable.Where<ICharacter>(x, (Func<ICharacter, bool>)((ICharacter character) => _storyManagerCluster.IsPhraseEnable(character.ID))), (Func<ICharacter, IObservable<CharacterStories>>)((ICharacter y) => _characterProvider.GetStory(y.ID)))))));
		IObservable<Unit> observable2 = Observable.AsUnitObservable<SkinData>(Observable.LastOrDefault<SkinData>(Observable.DefaultIfEmpty<SkinData>(Observable.Concat<SkinData>(from girl in source.OfType<SkinLinkedContent>()
			select girl.Skin.ID into id
			select TryLoadSkin(id, @event)))));
		return Observable.Select<Unit, Event>(Observable.LastOrDefault<Unit>(Observable.Merge<Unit>(observable, new IObservable<Unit>[1] { observable2 })).Debug($"Finished character and skin data loading for event {@event.EventId}", LogType.Events), (Func<Unit, Event>)((Unit _) => @event));
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
