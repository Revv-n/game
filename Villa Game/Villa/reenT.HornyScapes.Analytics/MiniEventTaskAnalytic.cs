using System;
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
		_gameStarter.IsGameActive.Where((bool x) => x).SelectMany((bool _) => EmitOnRewardTask()).Subscribe(base.SendEventIfIsValid)
			.AddTo(onNewStream);
	}

	private IObservable<Task> EmitOnRewardTask()
	{
		IObservable<bool> other = _gameStarter.IsGameActive.Where((bool x) => !x);
		return _miniEventTaskManager.Tasks.Where((MiniEventTask _minieventTask) => !_minieventTask.IsRewarded).ToObservable().SelectMany((MiniEventTask _minieventTask) => _minieventTask.OnUpdate)
			.TakeUntil(other);
	}

	protected override bool IsValid(Task entity)
	{
		return entity.IsRewarded;
	}

	public override void SendEventByPass(Task tuple)
	{
		MiniEventTask miniEventTask = tuple as MiniEventTask;
		MiniEventTaskAmplitudeEvent analyticsEvent = new MiniEventTaskAmplitudeEvent(miniEventTask.ID, miniEventTask.Identificator[0]);
		amplitude.AddEvent(analyticsEvent);
	}
}
