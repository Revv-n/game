using StripClub.NewEvent.Data;
using Zenject;

namespace GreenT.HornyScapes;

public class NotifyServiceWrapper<T> : IInitializable where T : ICalendarQueueListener
{
	private readonly CalendarQueue _calendarQueue;

	private T _queueListener;

	private NotifyServiceWrapper(CalendarQueue calendarQueue, T queueListener)
	{
		_calendarQueue = calendarQueue;
		_queueListener = queueListener;
	}

	public void Initialize()
	{
		_queueListener.Initialize(_calendarQueue);
	}
}
