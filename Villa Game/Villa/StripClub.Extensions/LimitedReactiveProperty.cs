using System;
using System.Runtime.Serialization;
using UniRx;

namespace StripClub.Extensions;

public class LimitedReactiveProperty : ReactiveProperty<int>
{
	private DateTime today;

	private Func<DateTime> getTime;

	private IDisposable watcher;

	public int Limit { get; private set; }

	public int Available { get; private set; }

	public bool Drop(int value)
	{
		if (Available < 1)
		{
			return false;
		}
		int available = Available - 1;
		Available = available;
		available = base.Value + 1;
		base.Value = available;
		return true;
	}

	public LimitedReactiveProperty(int value, int limit, int available, DateTime today)
		: base(value)
	{
		Limit = limit;
		Available = available;
		this.today = today;
	}

	public void SetLimit(int limit)
	{
		Available += Limit - limit;
		Limit = limit;
	}

	public void SetTimer(Func<DateTime> getTime)
	{
		this.getTime = getTime;
		watcher?.Dispose();
		watcher = Observable.Interval(TimeSpan.FromSeconds(1.0)).TakeWhile((long _) => getTime != null).Subscribe(delegate
		{
			ResetLimits();
		});
	}

	private void ResetLimits()
	{
		DateTime dateTime = getTime();
		if (dateTime.Day != today.Day || dateTime.Month != today.Month || dateTime.Year != today.Year)
		{
			today = dateTime;
			Available = Limit;
		}
	}

	public LimitedReactiveProperty(SerializationInfo info, StreamingContext context)
	{
		Limit = info.GetInt32("Limit");
		Available = info.GetInt32("Available");
		base.Value = info.GetInt32("Value");
		today = info.GetDateTime("Today");
	}

	public void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue("Today", today);
		info.AddValue("Limit", Limit);
		info.AddValue("Available", Available);
		info.AddValue("Value", base.Value);
	}
}
