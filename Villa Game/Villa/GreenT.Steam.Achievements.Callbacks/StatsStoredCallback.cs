using Steamworks;
using UnityEngine;

namespace GreenT.Steam.Achievements.Callbacks;

public sealed class StatsStoredCallback : BaseAchievementCallback, IHaveCallback
{
	private Callback<UserStatsStored_t> userStatsStored;

	private StatsReceivedCallback _statsReceivedCallback;

	public StatsStoredCallback(StatsReceivedCallback statsReceivedCallback)
	{
		_statsReceivedCallback = statsReceivedCallback;
	}

	public override void BindCallback()
	{
		userStatsStored = Callback<UserStatsStored_t>.Create(OnUserStatsStored);
	}

	private void OnUserStatsStored(UserStatsStored_t pCallback)
	{
		if ((ulong)gameId == pCallback.m_nGameID)
		{
			if (EResult.k_EResultOK == pCallback.m_eResult)
			{
				Debug.Log("StoreStats - success");
			}
			else if (EResult.k_EResultInvalidParam == pCallback.m_eResult)
			{
				Debug.Log("StoreStats - some failed to validate");
				UserStatsReceived_t pCallback2 = default(UserStatsReceived_t);
				pCallback2.m_eResult = EResult.k_EResultOK;
				pCallback2.m_nGameID = (ulong)gameId;
				_statsReceivedCallback.OnUserStatsReceived(pCallback2);
			}
			else
			{
				Debug.Log("StoreStats - failed, " + pCallback.m_eResult);
			}
		}
	}
}
