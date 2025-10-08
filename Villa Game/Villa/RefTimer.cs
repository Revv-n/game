using System;
using UnityEngine;

[Serializable]
public class RefTimer
{
	[SerializeField]
	protected RefDateTime start_time;

	[SerializeField]
	protected float time;

	public bool IsDefault => start_time.IsDefault;

	public virtual bool IsCompleted => start_time.Value.AddSeconds(time) < TimeMaster.Now;

	public virtual float Passed => (float)(TimeMaster.Now - start_time.Value).TotalSeconds;

	public virtual float Percent
	{
		get
		{
			if (time == 0f)
			{
				return 1f;
			}
			return Passed / time;
		}
	}

	public virtual float TimeLeft => time - Passed;

	public virtual float OverTime => Passed - time;

	public virtual float TotalTime => time;

	public virtual DateTime StartTime
	{
		get
		{
			return start_time.Value;
		}
		set
		{
			start_time.Value = value;
		}
	}

	public virtual DateTime EndTime => start_time.Value.AddSeconds(time);

	public virtual int PeriodsCount
	{
		get
		{
			if (time == 0f)
			{
				return 0;
			}
			return Mathf.FloorToInt(Passed / time);
		}
	}

	public RefTimer(float time)
	{
		this.time = time;
		start_time = RefDateTime.Now;
	}

	public RefTimer(float time, DateTime start)
	{
		this.time = time;
		start_time = new RefDateTime(start);
	}

	public virtual RefTimer Copy()
	{
		return new RefTimer(time)
		{
			start_time = start_time.Copy()
		};
	}

	public virtual int RemovePeriods()
	{
		int periodsCount = PeriodsCount;
		if (periodsCount == 0)
		{
			return 0;
		}
		start_time.Value = TimeMaster.Now.AddSeconds((0f - Passed) % time);
		return periodsCount;
	}
}
