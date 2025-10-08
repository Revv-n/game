using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Events;
using GreenT.Types;
using StripClub.NewEvent.Data;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.MergeStore;

public class SectionCluster : IDisposable, IInitializable
{
	private readonly DataManagerCluster _cluster;

	private readonly EventProvider _eventProvider;

	private StoreSection _regularMain;

	private StoreSection _premiumMain;

	private StoreSection _regularEvent;

	private StoreSection _premiumEvent;

	private readonly SectionFactory _sectionFactory;

	private readonly GameStarter _gameStarter;

	private readonly CompositeDisposable _disposable = new CompositeDisposable();

	private string EventBundleType => _eventProvider.CurrentCalendarProperty.Value.Item2.Bundle.Type;

	public SectionCluster(DataManagerCluster cluster, EventProvider eventProvider, SectionFactory sectionFactory, GameStarter gameStarter)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		_cluster = cluster;
		_eventProvider = eventProvider;
		_sectionFactory = sectionFactory;
		_gameStarter = gameStarter;
	}

	public void Initialize()
	{
		ObservableExtensions.Subscribe<bool>(Observable.Take<bool>(Observable.Where<bool>((IObservable<bool>)_gameStarter.IsGameActive, (Func<bool, bool>)((bool x) => x)), 1), (Action<bool>)delegate
		{
			InitializationAfterGameStart();
		});
	}

	private void InitializationAfterGameStart()
	{
		ObservableExtensions.Subscribe<ContentType>(_cluster.OnUpdate, (Action<ContentType>)Preload);
		_cluster.Initialization();
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<(CalendarModel, Event)>(Observable.Where<(CalendarModel, Event)>((IObservable<(CalendarModel, Event)>)_eventProvider.CurrentCalendarProperty, (Func<(CalendarModel, Event), bool>)(((CalendarModel calendar, Event @event) x) => x.@event != null)), (Action<(CalendarModel, Event)>)delegate
		{
			Preload(ContentType.Event);
		}), (ICollection<IDisposable>)_disposable);
	}

	private void Preload(ContentType contentType)
	{
		switch (contentType)
		{
		case ContentType.Main:
			TryInitializeMainSections();
			break;
		case ContentType.Event:
			if (_eventProvider.CurrentCalendarProperty.Value.Item2 != null)
			{
				TryInitializeEventSections(EventBundleType);
			}
			break;
		default:
			throw new ArgumentOutOfRangeException("contentType", contentType, null);
		}
	}

	public StorePreset GetPreset(ContentType contentType)
	{
		TryInitializeMainSections();
		return contentType switch
		{
			ContentType.Main => GetMainPreset(), 
			ContentType.Event => GetEventPreset(EventBundleType), 
			_ => throw new ArgumentOutOfRangeException("contentType", contentType, null), 
		};
	}

	public IEnumerable<StoreSection> GetEventSectionsToReset()
	{
		return new List<StoreSection> { _regularEvent, _premiumEvent }.Where((StoreSection mergeStoreSection) => mergeStoreSection != null);
	}

	public void ResetEventSection()
	{
		_regularEvent = null;
		_premiumEvent = null;
	}

	private StorePreset GetMainPreset()
	{
		return new StorePreset(_regularMain, _premiumMain);
	}

	private void TryInitializeMainSections()
	{
		string bundle = "Main";
		SectionCreateDataPreset sectionContainerForTypes = _cluster.GetSectionContainerForTypes(ContentType.Main, bundle);
		if (sectionContainerForTypes != null)
		{
			if (_regularMain == null)
			{
				_regularMain = _sectionFactory.Create(sectionContainerForTypes.Regular, bundle);
			}
			if (_premiumMain == null)
			{
				_premiumMain = _sectionFactory.Create(sectionContainerForTypes.Premium, bundle);
			}
		}
	}

	private void TryInitializeEventSections(string bundle)
	{
		SectionCreateDataPreset sectionContainerForTypes = _cluster.GetSectionContainerForTypes(ContentType.Event, bundle);
		if (sectionContainerForTypes != null)
		{
			if (_regularEvent == null)
			{
				_regularEvent = _sectionFactory.Create(sectionContainerForTypes.Regular, bundle);
			}
			else if (_regularEvent.ShopItems.Count == 0)
			{
				_sectionFactory.FixSection(_regularEvent, bundle);
			}
			if (sectionContainerForTypes.Premium != null && _premiumEvent == null)
			{
				_premiumEvent = _sectionFactory.Create(sectionContainerForTypes.Premium, bundle);
			}
		}
	}

	private StorePreset GetEventPreset(string bundleType)
	{
		TryInitializeEventSections(bundleType);
		StoreSection premium = _premiumEvent ?? _premiumMain;
		return new StorePreset(_regularEvent, premium);
	}

	public void Dispose()
	{
		_disposable.Dispose();
	}
}
