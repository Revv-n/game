using System;

namespace GreenT.Steam.Achievements.Goals.Objectives;

public abstract class TrackService<TData> : ITrackService where TData : struct, IComparable
{
	protected readonly AchievementService AchievementService;

	protected readonly AchievementDTO Achievement;

	public TData TargetValue { get; }

	public string StatsKey { get; }

	public TrackService(AchievementService achievementService, AchievementDTO achievement, string statsKey, TData targetValue)
	{
		AchievementService = achievementService;
		Achievement = achievement;
		TargetValue = targetValue;
		StatsKey = statsKey;
	}

	public virtual void Track()
	{
		throw new NotImplementedException();
	}

	protected bool IsComplete()
	{
		return AchievementService.IsComplete(Achievement);
	}
}
