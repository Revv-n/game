using System.Collections.Generic;
using GreenT.Types;
using StripClub.Model;

namespace GreenT.HornyScapes;

public sealed class Rating
{
	private readonly Matchmaking _matchmaking;

	public int ID { get; private set; }

	public int Range { get; private set; }

	public CurrencyType CurrencyType { get; private set; }

	public CompositeIdentificator CurrencyIdentificator { get; private set; }

	public Rating(int id, int range, CurrencyType currencyType, CompositeIdentificator currencyIdentificator, Matchmaking matchmaking)
	{
		ID = id;
		Range = range;
		CurrencyType = currencyType;
		CurrencyIdentificator = currencyIdentificator;
		_matchmaking = matchmaking;
	}

	public bool TryGetRewardForLevel(float playerPower, int level, out IEnumerable<LinkedContent> rewards)
	{
		return _matchmaking.TryGetRewardForLevel(playerPower, level, out rewards);
	}

	public bool TryGetLootboxRewardForLevel(float playerPower, int level, out LootboxLinkedContent reward)
	{
		return _matchmaking.TryGetLootboxRewardForLevel(playerPower, level, out reward);
	}
}
