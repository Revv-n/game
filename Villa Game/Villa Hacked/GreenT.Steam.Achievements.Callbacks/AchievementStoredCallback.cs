using Steamworks;
using UnityEngine;

namespace GreenT.Steam.Achievements.Callbacks;

public sealed class AchievementStoredCallback : BaseAchievementCallback
{
	private Callback<UserAchievementStored_t> userAchievementStored;

	public override void BindCallback()
	{
		userAchievementStored = Callback<UserAchievementStored_t>.Create(OnAchievementStored);
	}

	private void OnAchievementStored(UserAchievementStored_t pCallback)
	{
		if ((ulong)gameId == pCallback.m_nGameID)
		{
			if (pCallback.m_nMaxProgress == 0)
			{
				Debug.Log("Achievement '" + pCallback.m_rgchAchievementName + "' unlocked!");
				return;
			}
			Debug.Log("Achievement '" + pCallback.m_rgchAchievementName + "' progress callback, (" + pCallback.m_nCurProgress + "," + pCallback.m_nMaxProgress + ")");
		}
	}
}
