using System;
using System.Collections.Generic;
using GreenT.HornyScapes.BattlePassSpace;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Saves;

public class EventEndSaveEvent : SaveEvent
{
	private CalendarQueue _calendarQueue;

	[Inject]
	public void InnerInit(CalendarQueue calendarQueue)
	{
		_calendarQueue = calendarQueue;
	}

	public override void Track()
	{
		Initialize();
	}

	private void Initialize()
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<CalendarModel>(_calendarQueue.OnCalendarEnd(), (Action<CalendarModel>)delegate
		{
			Save();
		}), (ICollection<IDisposable>)saveStreams);
	}
}
