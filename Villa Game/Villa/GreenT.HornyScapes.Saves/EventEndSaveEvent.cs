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
		_calendarQueue.OnCalendarEnd().Subscribe(delegate
		{
			Save();
		}).AddTo(saveStreams);
	}
}
