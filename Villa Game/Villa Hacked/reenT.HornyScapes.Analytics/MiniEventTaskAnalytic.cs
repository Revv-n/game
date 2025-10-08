using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes;
using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.MiniEvents;
using GreenT.HornyScapes.Tasks;
using UniRx;

namespace reenT.HornyScapes.Analytics;

public sealed class MiniEventTaskAnalytic : BaseEntityAnalytic<Task>
{
	private readonly GameStarter _gameStarter;

	private readonly MiniEventTaskManager _miniEventTaskManager;

	public MiniEventTaskAnalytic(IAmplitudeSender<AmplitudeEvent> amplitude, GameStarter gameStarter, MiniEventTaskManager miniEventTaskManager)
		: base(amplitude)
	{
		_miniEventTaskManager = miniEventTaskManager;
		_gameStarter = gameStarter;
	}

	public override void Track()
	{
		ClearStreams();
		base.Track();
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Task>(Observable.SelectMany<bool, Task>(Observable.Where<bool>((IObservable<bool>)_gameStarter.IsGameActive, (Func<bool, bool>)((bool x) => x)), (Func<bool, IObservable<Task>>)((bool _) => EmitOnRewardTask())), (Action<Task>)base.SendEventIfIsValid), (ICollection<IDisposable>)onNewStream);
	}

	private IObservable<Task> EmitOnRewardTask()
	{
		IObservable<bool> observable = Observable.Where<bool>((IObservable<bool>)_gameStarter.IsGameActive, (Func<bool, bool>)((bool x) => !x));
		return Observable.TakeUntil<Task, bool>(Observable.SelectMany<MiniEventTask, Task>(Observable.ToObservable<MiniEventTask>(_miniEventTaskManager.Tasks.Where((MiniEventTask _minieventTask) => !_minieventTask.IsRewarded)), (Func<MiniEventTask, IObservable<Task>>)((MiniEventTask _minieventTask) => _minieventTask.OnUpdate)), observable);
	}

	protected override bool IsValid(Task entity)
	{
		return entity.IsRewarded;
	}

	public override void SendEventByPass(Task tuple)
	{
		MiniEventTask miniEventTask = tuple as MiniEventTask;
		MiniEventTaskAmplitudeEvent miniEventTaskAmplitudeEvent = new MiniEventTaskAmplitudeEvent(miniEventTask.ID, miniEventTask.Identificator[0]);
		((IAnalyticSender<AmplitudeEvent>)(object)amplitude).AddEvent((AmplitudeEvent)miniEventTaskAmplitudeEvent);
	}
}
