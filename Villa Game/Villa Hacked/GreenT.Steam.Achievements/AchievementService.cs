using System;
using Steamworks;

namespace GreenT.Steam.Achievements;

public class AchievementService
{
	public void GetStat(string statsKey, out int statsValue)
	{
		statsValue = 0;
		SteamUserStats.GetStat(statsKey, out statsValue);
	}

	public void GetStat(string statsKey, out float statsValue)
	{
		statsValue = 0f;
		SteamUserStats.GetStat(statsKey, out statsValue);
	}

	public void SetStat(string statsKey, int currentValue)
	{
		SteamUserStats.SetStat(statsKey, currentValue);
	}

	public void SetStat(string statsKey, float currentValue)
	{
		SteamUserStats.SetStat(statsKey, currentValue);
	}

	public bool IsComplete(AchievementDTO achievement)
	{
		bool pbAchieved = false;
		try
		{
			SteamUserStats.GetAchievement(achievement.AchievementIdType.ToString(), out pbAchieved);
		}
		catch (Exception exception)
		{
			exception.LogException();
		}
		return pbAchieved;
	}

	public void UnlockAchievement(AchievementDTO achievementDto)
	{
		achievementDto.Achieved = true;
		SteamUserStats.SetAchievement(achievementDto.AchievementIdType.ToString());
		StoreStats();
	}

	public void UpdateStats()
	{
		StoreStats();
	}

	private void StoreStats()
	{
		SteamUserStats.StoreStats();
	}
}
