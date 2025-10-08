using System;
using GreenT.HornyScapes.Events;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class MiniEventsStateTransitionHandler : IInitializable, IDisposable
{
	private readonly MiniEventsStateService _miniEventsStateService;

	private IDisposable _startTracker;

	private IDisposable _endTracker;

	public MiniEventsStateTransitionHandler(MiniEventsStateService miniEventsStateService)
	{
		_miniEventsStateService = miniEventsStateService;
	}

	public void Initialize()
	{
		_startTracker = ObservableExtensions.Subscribe<MiniEvent>(_miniEventsStateService.OnStart(), (Action<MiniEvent>)OnMiniEventStart);
		_endTracker = ObservableExtensions.Subscribe<MiniEvent>(_miniEventsStateService.OnEnd(), (Action<MiniEvent>)OnMiniEventEnd);
	}

	public void Dispose()
	{
		_startTracker.Dispose();
		_endTracker.Dispose();
	}

	private void OnMiniEventStart(MiniEvent minievent)
	{
		minievent?.InitializeControllers();
	}

	private void OnMiniEventEnd(MiniEvent minievent)
	{
		minievent?.DisposeControllers();
	}
}
