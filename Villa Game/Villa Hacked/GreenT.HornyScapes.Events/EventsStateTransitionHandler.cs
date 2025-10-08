using System;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Events;

public class EventsStateTransitionHandler : IInitializable, IDisposable
{
	private readonly EventsStateService _eventsStateService;

	private IDisposable _startTracker;

	private IDisposable _endTracker;

	public EventsStateTransitionHandler(EventsStateService eventsStateService)
	{
		_eventsStateService = eventsStateService;
	}

	public void Initialize()
	{
		_startTracker = ObservableExtensions.Subscribe<Event>(_eventsStateService.OnStart(), (Action<Event>)OnEventStart);
		_endTracker = ObservableExtensions.Subscribe<Event>(_eventsStateService.OnEnd(), (Action<Event>)OnEventEnd);
	}

	public void Dispose()
	{
		_startTracker.Dispose();
		_endTracker.Dispose();
	}

	private void OnEventStart(Event _event)
	{
		_event?.InitializeControllers();
	}

	private void OnEventEnd(Event _event)
	{
		_event?.DisposeControllers();
	}
}
