using System;
using UnityEngine;

namespace StripClub.Model.Data;

public abstract class ChainedPeriodTime : SaleTime, ISerializationCallbackReceiver
{
	[Tooltip("Total period duration")]
	[SerializeField]
	protected string periodDurationString;

	[SerializeField]
	protected ChainedPeriodTime next;

	protected TimeSpan periodDuration = TimeSpan.FromHours(1.0);

	public ChainedPeriodTime Next => next;

	public TimeSpan Duration => periodDuration;

	public virtual void OnValidate()
	{
		SaleTime.ValidateTimeSpan(ref periodDurationString, ref periodDuration, TimeSpan.FromHours(1.0));
		if (periodDuration <= TimeSpan.Zero)
		{
			Debug.LogError("Period of time must be greater than zero!");
			periodDuration = TimeSpan.FromHours(1.0);
			periodDurationString = periodDuration.ToString();
		}
		LoopDetector();
	}

	private void LoopDetector()
	{
		ChainedPeriodTime chainedPeriodTime = Next;
		while (chainedPeriodTime != null && chainedPeriodTime != this)
		{
			chainedPeriodTime = chainedPeriodTime.Next;
		}
		if (chainedPeriodTime == this)
		{
			Debug.LogError("Loop detected");
			next = null;
		}
	}

	public virtual void SetNext(ChainedPeriodTime next)
	{
		this.next = next;
	}

	public virtual void OnBeforeSerialize()
	{
		periodDurationString = periodDuration.ToString();
	}

	public virtual void OnAfterDeserialize()
	{
		TimeSpan.TryParse(periodDurationString, out periodDuration);
	}
}
