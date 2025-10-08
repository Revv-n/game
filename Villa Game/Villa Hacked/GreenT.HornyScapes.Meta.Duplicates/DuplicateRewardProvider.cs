using GreenT.HornyScapes.Characters.Skins.Content;
using GreenT.HornyScapes.Meta.Decorations;
using StripClub.Model;

namespace GreenT.HornyScapes.Meta.Duplicates;

public class DuplicateRewardProvider
{
	private readonly DuplicateRewardsManager _rewardsManager;

	public DuplicateRewardProvider(DuplicateRewardsManager rewardsManager)
	{
		_rewardsManager = rewardsManager;
	}

	public bool TryGetDuplicateReward(LinkedContent content, out LinkedContent reward)
	{
		if (!(content is DecorationLinkedContent decorationLinkedContent))
		{
			if (content is SkinLinkedContent skinLinkedContent)
			{
				reward = GetDuplicateReward(skinLinkedContent.Skin.ID);
				if (skinLinkedContent.Skin.IsOwned)
				{
					return reward != null;
				}
				return false;
			}
			reward = null;
			return false;
		}
		reward = GetDuplicateReward(decorationLinkedContent.Decoration.ID);
		if (decorationLinkedContent.Decoration.IsRewarded)
		{
			return reward != null;
		}
		return false;
	}

	private LinkedContent GetDuplicateReward(int id)
	{
		return _rewardsManager.GetDuplicateReward(id);
	}
}
