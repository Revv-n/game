using System;
using UniRx;

namespace StripClub.Extensions;

public static class TimeExtension
{
	public static TimeSpan Multiply(this TimeSpan timeSpan, long mul)
	{
		return TimeSpan.FromTicks(timeSpan.Ticks * mul);
	}

	public static DateTime ConvertFromUnixTimestamp(long timestamp)
	{
		return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(timestamp);
	}

	public static long ConvertToUnixTimestamp(this DateTime date)
	{
		DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
		return (long)(date.ToUniversalTime() - dateTime).TotalSeconds;
	}

	public static IObservable<long> TickTimerToZero(ReactiveProperty<TimeSpan> seconds)
	{
		return Observable.TakeWhile<long>(Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(1.0), Scheduler.MainThreadIgnoreTimeScale), (Func<long, bool>)((long x) => seconds.Value.TotalSeconds > 0.0));
	}

	public static DateTime ToDate(this long timeStamp)
	{
		return ConvertFromUnixTimestamp(timeStamp);
	}
}
