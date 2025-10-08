using System;
using UnityEngine;

namespace StripClub.Model.Data;

[CreateAssetMenu(menuName = "StripClub/Shop/Time/Chained/Head", order = 1)]
public class HeadPeriodTime : ChainedPeriodTime, ISerializationCallbackReceiver
{
	[SerializeField]
	protected string startTimeString;

	[SerializeField]
	protected string expirationTimeString;

	protected DateTime startTime = DateTime.MinValue;

	protected DateTime expirationTime = DateTime.MaxValue;

	[SerializeField]
	protected TimeSpan totalPeriodDuration;

	public override DateTime StartTime
	{
		get
		{
			if (SaleTime.CurrentTime.HasValue && SaleTime.CurrentTime > startTime)
			{
				long num = (((SaleTime.CurrentTime < expirationTime) ? SaleTime.CurrentTime.Value : expirationTime) - startTime).Ticks / totalPeriodDuration.Ticks;
				return startTime + totalPeriodDuration.Multiply((double)num);
			}
			Debug.LogWarning("Clock weren't set for SaleTime");
			return startTime;
		}
	}

	public override DateTime ExpirationTime => StartTime + base.Duration;

	public DateTime ChainExpirationTime => expirationTime;

	public void CalcTotalPeriodDuration()
	{
		ChainedPeriodTime chainedPeriodTime = base.Next;
		totalPeriodDuration = base.Duration;
		while (chainedPeriodTime != null)
		{
			totalPeriodDuration += chainedPeriodTime.Duration;
			if (chainedPeriodTime is NodePeriodTime)
			{
				totalPeriodDuration -= (chainedPeriodTime as NodePeriodTime).Offset;
			}
			chainedPeriodTime = chainedPeriodTime.Next;
		}
	}

	public override void OnValidate()
	{
		if (base.Next is NodePeriodTime)
		{
			(base.Next as NodePeriodTime).SetPrevious(this);
		}
		base.OnValidate();
	}

	public override void OnAfterDeserialize()
	{
		base.OnAfterDeserialize();
		DateTime.TryParse(startTimeString, out startTime);
		DateTime.TryParse(expirationTimeString, out expirationTime);
	}

	public override void OnBeforeSerialize()
	{
		base.OnBeforeSerialize();
		startTimeString = startTime.ToString();
		expirationTimeString = expirationTime.ToString();
	}
}
