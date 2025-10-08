using System;
using System.Linq;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.Tasks;
using GreenT.Types;
using UniRx;

namespace GreenT.HornyScapes.Analytics;

public class EventTaskAnalytic : BaseEntityAnalytic<Task>
{
	private readonly GameStarter gameStarter;

	private readonly TaskManagerCluster taskManagerCluster;

	private readonly CalendarQueue calendarQueue;

	public EventTaskAnalytic(IAmplitudeSender<AmplitudeEvent> amplitude, CalendarQueue calendarQueue, GameStarter gameStarter, TaskManagerCluster taskManagerCluster)
		: base(amplitude)
	{
		this.gameStarter = gameStarter;
		this.taskManagerCluster = taskManagerCluster;
		this.calendarQueue = calendarQueue;
	}

	public override void Track()
	{
		ClearStreams();
		base.Track();
		gameStarter.IsGameActive.Where((bool x) => x).SelectMany((bool _) => EmitOnRewardTask()).Subscribe(base.SendEventIfIsValid)
			.AddTo(onNewStream);
	}

	private IObservable<Task> EmitOnRewardTask()
	{
		IObservable<bool> other = gameStarter.IsGameActive.Where((bool x) => !x);
		return taskManagerCluster[ContentType.Event].Tasks.Where((Task _eventTask) => !_eventTask.IsRewarded).ToObservable().SelectMany((Task _eventTask) => _eventTask.OnUpdate)
			.TakeUntil(other);
	}

	protected override bool IsValid(Task entity)
	{
		return entity.IsRewarded;
	}

	public override void SendEventByPass(Task tuple)
	{
		EventTaskAmplitudeEvent analyticsEvent = new EventTaskAmplitudeEvent(tuple.ID, calendarQueue.GetActiveCalendar(EventStructureType.Event).BalanceId);
		amplitude.AddEvent(analyticsEvent);
	}
}
