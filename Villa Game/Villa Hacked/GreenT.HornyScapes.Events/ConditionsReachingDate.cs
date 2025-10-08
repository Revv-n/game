using System;
using System.Globalization;
using StripClub.Extensions;
using StripClub.UI;
using UniRx;

namespace GreenT.HornyScapes.Events;

public class ConditionsReachingDate : BaseConditionReceivingReward<ConditionsReachingDate>
{
	private readonly DateTime targetDate;

	private readonly IClock clock;

	private readonly GenericTimer Timer = new GenericTimer();

	public override string ConditionText => GetStringTime();

	public ConditionsReachingDate(long targetDate, IClock clock)
	{
		this.targetDate = UnixTimeStampToDateTime(targetDate);
		this.clock = clock;
	}

	public override bool Validate()
	{
		return ValidateTime();
	}

	protected override void Subscribe()
	{
		Timer.Start(GetTimeSpan());
		SubscribeDispose = ObservableExtensions.Subscribe<TimeSpan>(Timer.OnUpdate, (Action<TimeSpan>)delegate
		{
			ValidateTime();
		});
	}

	protected override bool CheckIfCompleted()
	{
		if (!TimeIsCompleted())
		{
			return false;
		}
		SetCompleted();
		return true;
	}

	private bool ValidateTime()
	{
		if (IsDisabled())
		{
			return false;
		}
		if (IsCompleted())
		{
			return true;
		}
		bool num = TimeIsCompleted();
		if (num && !IsCompleted())
		{
			SetCompleted();
		}
		return num;
	}

	private bool TimeIsCompleted()
	{
		return GetTimeSpan() <= TimeSpan.Zero;
	}

	private TimeSpan GetTimeSpan()
	{
		return targetDate - clock.GetTime();
	}

	private static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
	{
		return TimeExtension.ConvertFromUnixTimestamp(unixTimeStamp);
	}

	private string GetStringTime()
	{
		string shortDatePattern = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
		return targetDate.ToString(shortDatePattern);
	}
}
