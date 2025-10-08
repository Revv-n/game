using Steamworks;

namespace GreenT.Steam.Achievements.Callbacks;

public class BindCallbackService
{
	private AchievementCallbackProvider _achievementCallbackProvider;

	public BindCallbackService(AchievementCallbackProvider achievementCallbackProvider)
	{
		_achievementCallbackProvider = achievementCallbackProvider;
	}

	public void Bind()
	{
		CGameID gameId = new CGameID(SteamUtils.GetAppID());
		foreach (BaseAchievementCallback item in _achievementCallbackProvider.Collection)
		{
			item.Initialize(gameId);
		}
	}
}
