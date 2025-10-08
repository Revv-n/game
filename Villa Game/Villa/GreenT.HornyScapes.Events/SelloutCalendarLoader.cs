using System;
using System.Collections.Generic;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Sellouts.Factories;
using GreenT.HornyScapes.Sellouts.Loaders;
using GreenT.HornyScapes.Sellouts.Mappers;
using GreenT.HornyScapes.Sellouts.Models;
using GreenT.HornyScapes.Sellouts.Providers;
using GreenT.HornyScapes.Sellouts.Services;
using UniRx;

namespace GreenT.HornyScapes.Events;

public class SelloutCalendarLoader : ICalendarLoader
{
	private readonly SelloutFactory _selloutFactory;

	private readonly SelloutMapperProvider _selloutMapperProvider;

	private readonly SelloutDataLoader _selloutDataLoader;

	private readonly SelloutManager _selloutManager;

	private readonly SelloutStateManager _selloutStateManager;

	private readonly CalendarStrategyFactory _calendarStrategyFactory;

	private readonly BundlesProviderBase _bundlesProvider;

	private readonly CompositeDisposable _calendarLoadDisposable = new CompositeDisposable();

	private readonly Dictionary<CalendarModel, IDisposable> _stateMap = new Dictionary<CalendarModel, IDisposable>();

	private readonly Subject<(CalendarModel, CalendarLoadingStatus)> _onLoadingStateChangeSubject = new Subject<(CalendarModel, CalendarLoadingStatus)>();

	public SelloutCalendarLoader(SelloutFactory selloutFactory, SelloutMapperProvider selloutMapperProvider, SelloutDataLoader selloutDataLoader, SelloutManager selloutManager, SelloutStateManager selloutStateManager, CalendarStrategyFactory calendarStrategyFactory, BundlesProviderBase bundlesProvider)
	{
		_selloutFactory = selloutFactory;
		_selloutMapperProvider = selloutMapperProvider;
		_selloutDataLoader = selloutDataLoader;
		_selloutManager = selloutManager;
		_selloutStateManager = selloutStateManager;
		_calendarStrategyFactory = calendarStrategyFactory;
		_bundlesProvider = bundlesProvider;
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
		IEventMapper eventMapper = calendarModel.EventMapper;
		SelloutMapper selloutMapper = eventMapper as SelloutMapper;
		if (selloutMapper == null)
		{
			return Observable.Empty<CalendarModel>();
		}
		SetLoadingStream(calendarModel);
		calendarModel.SetLoadingStatus(CalendarModelLoadingState.InProgress);
		calendarModel.SetLoadingLifeStatus(CalendarLoadingStatus.Start);
		IConnectableObservable<Sellout> connectableObservable = (from data in _selloutDataLoader.Load(selloutMapper.Bundle)
			select CreateSellout(calendarModel, selloutMapper, data)).Do(delegate(Sellout sellout)
		{
			ActivateSellout(sellout);
		}).Catch(delegate(Exception exception)
		{
			calendarModel.SetLoadingStatus(CalendarModelLoadingState.Failed);
			throw exception.SendException(string.Format("{0}: Error when creating {1} with structure: {2}\n", GetType().Name, "Sellout", EventStructureType.Sellout));
		}).Publish();
		IConnectableObservable<CalendarModel> connectableObservable2 = (from _ in connectableObservable.AsUnitObservable().LastOrDefault()
			select calendarModel).Do(delegate
		{
			calendarModel.SetLoadingLifeStatus(CalendarLoadingStatus.End);
			Remove(calendarModel);
		}).Publish();
		connectableObservable.Connect().AddTo(_calendarLoadDisposable);
		connectableObservable2.Connect().AddTo(_calendarLoadDisposable);
		return connectableObservable2;
	}

	public IEventMapper GetEventMapper(int event_id)
	{
		return _selloutMapperProvider.Get(event_id);
	}

	private Sellout CreateSellout(CalendarModel calendarModel, SelloutMapper selloutMapper, SelloutBundleData data)
	{
		try
		{
			Sellout sellout = _selloutFactory.Create(selloutMapper);
			sellout.Set(data);
			ICalendarStateStrategy calendarStateStrategy = _calendarStrategyFactory.Create(calendarModel.EventType, calendarModel);
			calendarModel.Set(sellout, calendarStateStrategy, sellout.ID);
			calendarModel.SetLoadingStatus(CalendarModelLoadingState.Success);
			return sellout;
		}
		catch (Exception innerException)
		{
			calendarModel.SetLoadingStatus(CalendarModelLoadingState.Failed);
			throw innerException.SendException(string.Format("{0}: Error when creating {1} with structure: {2}\n", GetType().Name, "Sellout", EventStructureType.Sellout));
		}
	}

	private void ActivateSellout(Sellout sellout)
	{
		_selloutManager.Add(sellout);
		_selloutStateManager.ActivateSellout(sellout);
	}
}
