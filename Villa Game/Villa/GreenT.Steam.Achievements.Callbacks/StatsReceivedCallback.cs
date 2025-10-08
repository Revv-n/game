using Steamworks;
using UnityEngine;

namespace GreenT.Steam.Achievements.Callbacks;

public sealed class StatsReceivedCallback : BaseAchievementCallback, IHaveCallback
{
	private readonly AchievementProvider _achievementProvider;

	private readonly AchievementService _achievementService;

	private Callback<UserStatsReceived_t> userStatsReceived;

	public StatsReceivedCallback(AchievementProvider achievementProvider)
	{
		_achievementProvider = achievementProvider;
	}

	public override void BindCallback()
	{
		userStatsReceived = Callback<UserStatsReceived_t>.Create(OnUserStatsReceived);
	}

	public void OnUserStatsReceived(UserStatsReceived_t pCallback)
	{
		if (!SteamState.Initialized || (ulong)gameId != pCallback.m_nGameID || EResult.k_EResultOK != pCallback.m_eResult)
		{
			return;
		}
		_achievementService.UpdateStats();
		AchievementDTO[] collection = _achievementProvider.Collection;
		foreach (AchievementDTO achievementDTO in collection)
		{
			if (SteamUserStats.GetAchievement(achievementDTO.AchievementIdType.ToString(), out achievementDTO.Achieved))
			{
				achievementDTO.Name = SteamUserStats.GetAchievementDisplayAttribute(achievementDTO.AchievementIdType.ToString(), "name");
				achievementDTO.Description = SteamUserStats.GetAchievementDisplayAttribute(achievementDTO.AchievementIdType.ToString(), "desc");
			}
			else
			{
				Debug.LogWarning("SteamUserStats.GetAchievement failed for Achievement " + achievementDTO.AchievementIdType.ToString() + "\nIs it registered in the Steam Partner site?");
			}
		}
	}
}
