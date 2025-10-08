namespace StripClub.Model;

public class RelationshipRewardedLocker : Locker
{
	public int RelationshipdId { get; }

	public int RewardId { get; }

	public RelationshipRewardedLocker(int relationshipdId, int rewardId)
	{
		RelationshipdId = relationshipdId;
		RewardId = rewardId;
	}

	public void Set(int rewardId)
	{
		if (RewardId == rewardId)
		{
			isOpen.Value = true;
		}
	}
}
