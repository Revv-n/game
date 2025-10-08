using System.Linq;
using GreenT.Model.Collections;
using StripClub.Model;

namespace GreenT.HornyScapes.Meta.Duplicates;

public class DuplicateRewardsManager : SimpleManager<DuplicateReward>
{
	public LinkedContent GetDuplicateReward(int id)
	{
		return collection.FirstOrDefault((DuplicateReward reward) => reward.DuplicateID == id)?.GetReward();
	}
}
