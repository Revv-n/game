using System;
using System.Collections.Generic;
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
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Task>(Observable.SelectMany<bool, Task>(Observable.Where<bool>((IObservable<bool>)gameStarter.IsGameActive, (Func<bool, bool>)((bool x) => x)), (Func<bool, IObservable<Task>>)((bool _) => EmitOnRewardTask())), (Action<Task>)base.SendEventIfIsValid), (ICollection<IDisposable>)onNewStream);
	}

	private IObservable<Task> EmitOnRewardTask()
	{
		IObservable<bool> observable = Observable.Where<bool>((IObservable<bool>)gameStarter.IsGameActive, (Func<bool, bool>)((bool x) => !x));
		return Observable.TakeUntil<Task, bool>(Observable.SelectMany<Task, Task>(Observable.ToObservable<Task>(taskManagerCluster[ContentType.Event].Tasks.Where((Task _eventTask) => !_eventTask.IsRewarded)), (Func<Task, IObservable<Task>>)((Task _eventTask) => _eventTask.OnUpdate)), observable);
	}

	protected override bool IsValid(Task entity)
	{
		return entity.IsRewarded;
	}

	public override void SendEventByPass(Task tuple)
	{
		EventTaskAmplitudeEvent eventTaskAmplitudeEvent = new EventTaskAmplitudeEvent(tuple.ID, calendarQueue.GetActiveCalendar(EventStructureType.Event).BalanceId);
		((IAnalyticSender<AmplitudeEvent>)(object)amplitude).AddEvent((AmplitudeEvent)eventTaskAmplitudeEvent);
	}
}
