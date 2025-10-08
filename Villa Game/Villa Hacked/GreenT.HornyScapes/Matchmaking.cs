using System.Collections.Generic;
using System.Linq;
using StripClub.Model;

namespace GreenT.HornyScapes;

public class Matchmaking
{
	private Dictionary<Range, Dictionary<Range, IEnumerable<LinkedContent>>> _rewards;

	private Dictionary<Range, Dictionary<Range, LootboxLinkedContent>> _lootboxRewards;

	public int ID { get; private set; }

	public Matchmaking(int id)
	{
		ID = id;
		_rewards = new Dictionary<Range, Dictionary<Range, IEnumerable<LinkedContent>>>();
		_lootboxRewards = new Dictionary<Range, Dictionary<Range, LootboxLinkedContent>>();
	}

	public void TryAddNewRewardsRange(Range playerPower, Dictionary<Range, IEnumerable<LinkedContent>> rewards)
	{
		_rewards.TryAdd(playerPower, rewards);
	}

	public void TryAddNewLootboxRange(Range playerPower, Dictionary<Range, LootboxLinkedContent> lootboxRewards)
	{
		_lootboxRewards.TryAdd(playerPower, lootboxRewards);
	}

	public bool TryGetRewardForLevel(float playerPower, int level, out IEnumerable<LinkedContent> rewards)
	{
		if (_rewards.TryGetValue(_rewards.Keys.FirstOrDefault((Range key) => playerPower >= key.LowerBorder && playerPower <= key.UpperBorder), out var value))
		{
			value.TryGetValue(value.Keys.FirstOrDefault((Range key) => (float)level >= key.LowerBorder && (float)level <= key.UpperBorder), out rewards);
		}
		else
		{
			rewards = null;
		}
		return rewards != null;
	}

	public bool TryGetLootboxRewardForLevel(float playerPower, int level, out LootboxLinkedContent rewards)
	{
		if (_lootboxRewards.TryGetValue(_lootboxRewards.Keys.FirstOrDefault((Range key) => playerPower >= key.LowerBorder && playerPower <= key.UpperBorder), out var value))
		{
			value.TryGetValue(value.Keys.FirstOrDefault((Range key) => (float)level >= key.LowerBorder && (float)level <= key.UpperBorder), out rewards);
		}
		else
		{
			rewards = null;
		}
		return rewards != null;
	}
}
