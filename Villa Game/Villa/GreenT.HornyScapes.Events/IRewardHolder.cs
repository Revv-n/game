using System.Collections.Generic;
using Merge.Meta.RoomObjects;
using StripClub.Model;

namespace GreenT.HornyScapes.Events;

public interface IRewardHolder
{
	bool HasRewards();

	IEnumerable<LinkedContent> GetAllRewardsContent();

	IEnumerable<LinkedContent> GetUncollectedRewardsContent();

	int GetFilteredRewardsCount(IEnumerable<EntityStatus> states);
}
