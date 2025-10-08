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

	private string EventBundleType => _eventProvider.CurrentCalendarProperty.Value.@event.Bundle.Type;

	public SectionCluster(DataManagerCluster cluster, EventProvider eventProvider, SectionFactory sectionFactory, GameStarter gameStarter)
	{
		_cluster = cluster;
		_eventProvider = eventProvider;
		_sectionFactory = sectionFactory;
		_gameStarter = gameStarter;
	}

	public void Initialize()
	{
		_gameStarter.IsGameActive.Where((bool x) => x).Take(1).Subscribe(delegate
		{
			InitializationAfterGameStart();
		});
	}

	private void InitializationAfterGameStart()
	{
		_cluster.OnUpdate.Subscribe(Preload);
		_cluster.Initialization();
		_eventProvider.CurrentCalendarProperty.Where(((CalendarModel calendar, Event @event) x) => x.@event != null).Subscribe(delegate
		{
			Preload(ContentType.Event);
		}).AddTo(_disposable);
	}

	private void Preload(ContentType contentType)
	{
		switch (contentType)
		{
		case ContentType.Main:
			TryInitializeMainSections();
			break;
		case ContentType.Event:
			if (_eventProvider.CurrentCalendarProperty.Value.@event != null)
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
