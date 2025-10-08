using Steamworks;

namespace GreenT.Steam.Achievements.Callbacks;

public abstract class BaseAchievementCallback : IHaveCallback
{
	protected CGameID gameId;

	public void Initialize(CGameID gameId)
	{
		this.gameId = gameId;
	}

	public virtual void BindCallback()
	{
	}
}
