using System;
using System.Collections.Generic;
using System.Linq;

namespace Merge;

public class SpeedUpUnit
{
	private Dictionary<string, List<ISpeedUpSource>> speedUpSources = new Dictionary<string, List<ISpeedUpSource>>();

	public float CurrentSpeedUpValue { get; private set; } = 1f;


	public RefSkipableTimer CacedRefTimer { get; private set; }

	public TimerBase CacedTimer { get; private set; }

	public SpeedUpUnit(RefSkipableTimer refTimer, TimerBase tweenTimer, params ISpeedUpSource[] sources)
	{
		CacedRefTimer = refTimer;
		SetTimer(tweenTimer);
		if (sources != null)
		{
			foreach (ISpeedUpSource source in sources)
			{
				AddSpeedUpSource(source);
			}
		}
	}

	public void SetTimer(TimerBase timer)
	{
		if (CacedTimer != null)
		{
			CacedTimer.TickCallback -= AtTimerTick;
		}
		CacedTimer = timer;
		if (CacedTimer != null)
		{
			CacedTimer.TickCallback += AtTimerTick;
		}
	}

	public void AddSpeedUpSource(ISpeedUpSource source)
	{
		string key = source.SpeedUpInfo.Key;
		if (!speedUpSources.ContainsKey(key))
		{
			speedUpSources.Add(key, new List<ISpeedUpSource>());
		}
		speedUpSources[key].Add(source);
		ValidateSpeedUpMultiplyer();
	}

	public void RemoveSpeedUpSource(ISpeedUpSource source)
	{
		string key = source.SpeedUpInfo.Key;
		if (speedUpSources.ContainsKey(key))
		{
			speedUpSources[key].Remove(source);
			if (speedUpSources[key].Count == 0)
			{
				speedUpSources.Remove(key);
			}
			ValidateSpeedUpMultiplyer();
		}
	}

	public void RemoveSpeedUpSourcesWithKey(string key)
	{
		if (speedUpSources.ContainsKey(key))
		{
			speedUpSources.Remove(key);
			ValidateSpeedUpMultiplyer();
		}
	}

	public SpeedUpUnitData GetSave()
	{
		SpeedUpUnitData speedUpUnitData = new SpeedUpUnitData
		{
			BecomesOffline = RefDateTime.Now
		};
		foreach (KeyValuePair<string, List<ISpeedUpSource>> speedUpSource in speedUpSources)
		{
			speedUpUnitData.Sources.Add(speedUpSource.Key, new List<SpeedUpSourceInfo>());
			foreach (ISpeedUpSource item in speedUpSource.Value)
			{
				speedUpUnitData.Sources[speedUpSource.Key].Add(item.SpeedUpInfo);
			}
		}
		return speedUpUnitData;
	}

	public float GetOfflineTimeSkipped(SpeedUpUnitData save)
	{
		if (!save.Sources.ContainsKey("Tesla"))
		{
			return 0f;
		}
		SpeedUpSourceInfo speedUpSourceInfo = save.Sources["Tesla"].OrderByDescending((SpeedUpSourceInfo x) => x.Multiplier).First();
		return (float)((speedUpSourceInfo.EndTime.IsDefault ? TimeMaster.Now : ((DateTime.Compare(speedUpSourceInfo.EndTime.Value, TimeMaster.Now) < 0) ? speedUpSourceInfo.EndTime.Value : TimeMaster.Now)) - save.BecomesOffline.Value).TotalSeconds * (speedUpSourceInfo.Multiplier - 1f);
	}

	private void ValidateSpeedUpMultiplyer()
	{
		if (speedUpSources.Count == 0)
		{
			CurrentSpeedUpValue = 1f;
			return;
		}
		CurrentSpeedUpValue = 0f;
		foreach (List<ISpeedUpSource> value in speedUpSources.Values)
		{
			CurrentSpeedUpValue += value.Max((ISpeedUpSource x) => x.SpeedUpInfo.Multiplier);
		}
	}

	private void AtTimerTick(TimerStatus obj)
	{
		if (CurrentSpeedUpValue != 1f)
		{
			CacedRefTimer.Skipped += obj.Delta * (CurrentSpeedUpValue - 1f) / CurrentSpeedUpValue;
		}
	}
}
