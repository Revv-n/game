using System.Collections.Generic;
using GreenT.HornyScapes.MergeCore;
using Merge;

namespace GreenT.Steam.Achievements.Goals;

public class MergeTrackService : ITrackService
{
	private readonly AchievementService _achievementService;

	private readonly string _statsKey;

	private List<MergeData> collection = new List<MergeData>();

	public MergeTrackService(AchievementService achievementService, string statsKey)
	{
		_achievementService = achievementService;
		_statsKey = statsKey;
	}

	public void Add(AchievementDTO achievement, int targetValue)
	{
		collection.Add(new MergeData(achievement, targetValue));
	}

	public void Track()
	{
		Controller<MergeController>.Instance.OnMerge += UpdateStats;
	}

	private void UpdateStats(GameItem obj)
	{
		_achievementService.GetStat(_statsKey, out int statsValue);
		statsValue++;
		_achievementService.SetStat(_statsKey, statsValue);
		foreach (MergeData item in collection)
		{
			if (statsValue >= item.TargetValue)
			{
				_achievementService.UnlockAchievement(item.Achievement);
			}
		}
		_achievementService.UpdateStats();
	}
}
