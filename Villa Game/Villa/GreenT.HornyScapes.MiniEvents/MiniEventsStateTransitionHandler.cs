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
		_startTracker = _miniEventsStateService.OnStart().Subscribe(OnMiniEventStart);
		_endTracker = _miniEventsStateService.OnEnd().Subscribe(OnMiniEventEnd);
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
