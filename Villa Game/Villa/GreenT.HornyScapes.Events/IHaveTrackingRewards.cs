using System.Collections.Generic;

namespace GreenT.HornyScapes.Events;

public interface IHaveTrackingRewards
{
	IEnumerable<BaseReward> GetRewardsToTrack();
}
