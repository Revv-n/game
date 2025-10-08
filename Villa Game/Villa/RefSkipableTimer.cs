using System;
using UnityEngine;

[Serializable]
public class RefSkipableTimer : RefTimer
{
	[SerializeField]
	protected float skipped;

	public float Skipped
	{
		get
		{
			return skipped;
		}
		set
		{
			skipped = value;
		}
	}

	public override bool IsCompleted => start_time.Value.AddSeconds(time + skipped) < TimeMaster.Now;

	public override float Passed => (float)(TimeMaster.Now - start_time.Value).TotalSeconds + skipped;

	public override DateTime EndTime => start_time.Value.AddSeconds(time - skipped);

	public RefSkipableTimer(float time)
		: base(time)
	{
	}

	public RefSkipableTimer(float time, DateTime start)
		: base(time, start)
	{
	}

	public RefSkipableTimer(RefTimer baseTimer)
		: base(baseTimer.TotalTime, baseTimer.StartTime)
	{
	}

	public override RefTimer Copy()
	{
		return new RefSkipableTimer(time)
		{
			start_time = start_time.Copy(),
			skipped = skipped
		};
	}
}
