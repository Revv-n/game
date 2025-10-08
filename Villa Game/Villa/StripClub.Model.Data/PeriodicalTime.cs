using System;
using UnityEngine;

namespace StripClub.Model.Data;

[CreateAssetMenu(menuName = "StripClub/Shop/Time/PeriodicalTime", order = 2)]
public class PeriodicalTime : FiniteSaleTime
{
	[Tooltip("Total period duration")]
	[SerializeField]
	protected string periodDurationString;

	[Tooltip("Period of time when is active")]
	[SerializeField]
	protected string activeDurationString;

	[SerializeField]
	protected TimeSpan totalPeriodDuration;

	[SerializeField]
	protected TimeSpan activeDuration;

	public override DateTime StartTime
	{
		get
		{
			if (SaleTime.CurrentTime.HasValue && SaleTime.CurrentTime > startTime)
			{
				long num = (((SaleTime.CurrentTime < expirationTime) ? SaleTime.CurrentTime.Value : expirationTime) - startTime).Ticks / totalPeriodDuration.Ticks;
				return StartTime + totalPeriodDuration.Multiply(num);
			}
			Debug.LogWarning("Clock weren't set for SaleTime");
			return startTime;
		}
	}

	public override DateTime ExpirationTime
	{
		get
		{
			DateTime dateTime = StartTime + totalPeriodDuration;
			if (!(dateTime < expirationTime))
			{
				return expirationTime;
			}
			return dateTime;
		}
	}

	public override bool IsInclude(DateTime currentTime)
	{
		long num = (currentTime - startTime).Ticks / totalPeriodDuration.Ticks;
		_ = startTime + totalPeriodDuration.Multiply(num) + totalPeriodDuration;
		if (StartTime <= currentTime)
		{
			return currentTime <= StartTime + activeDuration;
		}
		return false;
	}

	protected override void OnValidate()
	{
		base.OnValidate();
		SaleTime.ValidateTimeSpan(ref periodDurationString, ref totalPeriodDuration);
		SaleTime.ValidateTimeSpan(ref activeDurationString, ref activeDuration);
		if (activeDuration > totalPeriodDuration)
		{
			activeDuration = totalPeriodDuration;
			activeDurationString = activeDuration.ToString();
			Debug.LogError("Active duration can't be more than total period duration");
		}
	}
}
