using System;
using System.Diagnostics;
using UnityEngine;

namespace StripClub.Model.Data;

public abstract class SaleTime : ScriptableObject, ISaleTime
{
	private static Func<DateTime> Clock;

	public static DateTime? CurrentTime => Clock?.Invoke();

	public abstract DateTime StartTime { get; }

	public abstract DateTime ExpirationTime { get; }

	public static void SetClock(Func<DateTime> clock)
	{
		Clock = clock;
	}

	public virtual bool IsInclude(DateTime currentTime)
	{
		if (StartTime < currentTime)
		{
			return currentTime < ExpirationTime;
		}
		return false;
	}

	[Conditional("UNITY_EDITOR")]
	protected static void ValidateDateTime(ref string dateString, ref DateTime date, DateTime defaultValue = default(DateTime), ScriptableObject obj = null)
	{
	}

	protected static void ValidateTimeSpan(ref string timeSpanString, ref TimeSpan timeSpan, TimeSpan defaultValue = default(TimeSpan))
	{
		if (string.IsNullOrEmpty(timeSpanString) || !TimeSpan.TryParse(timeSpanString, out var result))
		{
			UnityEngine.Debug.LogError("Wrong time format: \"" + timeSpanString + "\"");
			timeSpan = defaultValue;
			timeSpanString = timeSpan.ToString();
		}
		else
		{
			timeSpan = result;
		}
	}
}
