using System.Collections.Generic;
using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.Lootboxes;
using GreenT.Types;
using StripClub.Model;
using Zenject;

namespace GreenT.HornyScapes;

public sealed class MatchmakingFactory : IFactory<MatchmakingMapper, Matchmaking>, IFactory
{
	private readonly LinkedContentFactory _linkedContentFactory;

	private readonly LinkedContentAnalyticDataFactory _analyticDataFactory;

	private readonly MatchmakingManager _matchmakingManager;

	public MatchmakingFactory(LinkedContentFactory linkedContentFactory, LinkedContentAnalyticDataFactory analyticDataFactory, MatchmakingManager matchmakingManager)
	{
		_linkedContentFactory = linkedContentFactory;
		_analyticDataFactory = analyticDataFactory;
		_matchmakingManager = matchmakingManager;
	}

	public Matchmaking Create(MatchmakingMapper mapper)
	{
		Matchmaking matchmaking = _matchmakingManager.GetMatchmakingInfo(mapper.id);
		Range range = default(Range);
		range.LowerBorder = mapper.player_power_from;
		range.UpperBorder = mapper.player_power_to;
		Range playerPower = range;
		bool flag = matchmaking == null;
		List<Range> list = new List<Range>();
		for (int i = 0; i < mapper.range_value.Length; i++)
		{
			list.Add(ParseRange(mapper.range_value[i].Trim('[', ']')));
		}
		Dictionary<Range, IEnumerable<LinkedContent>> dictionary = new Dictionary<Range, IEnumerable<LinkedContent>>();
		Dictionary<Range, LootboxLinkedContent> dictionary2 = new Dictionary<Range, LootboxLinkedContent>();
		for (int j = 0; j < list.Count; j++)
		{
			LootboxLinkedContent lootboxLinkedContent = CreateLinkedContent(RewType.Lootbox, mapper.range_rewards[j]);
			dictionary2.Add(list[j], lootboxLinkedContent);
			dictionary.Add(list[j], CreateFullContent(lootboxLinkedContent));
		}
		if (flag)
		{
			matchmaking = new Matchmaking(mapper.id);
		}
		matchmaking.TryAddNewRewardsRange(playerPower, dictionary);
		matchmaking.TryAddNewLootboxRange(playerPower, dictionary2);
		if (!flag)
		{
			return null;
		}
		return matchmaking;
	}

	private Range ParseRange(string value)
	{
		string[] array = value.Split(':');
		if (array.Length == 2 && int.TryParse(array[0], out var result) && int.TryParse(array[1], out var result2))
		{
			Range result3 = default(Range);
			result3.LowerBorder = result;
			result3.UpperBorder = result2;
			return result3;
		}
		if (array.Length == 1 && int.TryParse(array[0], out var result4))
		{
			Range result3 = default(Range);
			result3.LowerBorder = result4;
			result3.UpperBorder = result4;
			return result3;
		}
		return default(Range);
	}

	private IEnumerable<LinkedContent> CreateFullContent(LootboxLinkedContent lootboxLinkedContent)
	{
		List<LinkedContent> list = new List<LinkedContent>();
		for (int i = 0; i < lootboxLinkedContent.Lootbox.GuarantedDrop.Count; i++)
		{
			LinkedContentAnalyticData analyticData = _analyticDataFactory.Create(CurrencyAmplitudeAnalytic.SourceType.MiniEventRating);
			LinkedContent linkedContent = _linkedContentFactory.Create(lootboxLinkedContent.Lootbox.GuarantedDrop[i].Type, lootboxLinkedContent.Lootbox.GuarantedDrop[i].Selector, lootboxLinkedContent.Lootbox.GuarantedDrop[i].Quantity, 0, ContentType.Main, analyticData);
			if (lootboxLinkedContent.Lootbox.GuarantedDrop[i].Type == RewType.Lootbox)
			{
				for (int j = 1; j < lootboxLinkedContent.Lootbox.GuarantedDrop[i].Quantity; j++)
				{
					LinkedContent content = _linkedContentFactory.Create(lootboxLinkedContent.Lootbox.GuarantedDrop[i].Type, lootboxLinkedContent.Lootbox.GuarantedDrop[i].Selector, lootboxLinkedContent.Lootbox.GuarantedDrop[i].Quantity, 0, ContentType.Main, analyticData);
					linkedContent.Insert(content);
				}
			}
			list.Add(linkedContent);
		}
		return list;
	}

	private LootboxLinkedContent CreateLinkedContent(RewType rewType, string reward_id)
	{
		Selector selector = SelectorTools.CreateSelector(reward_id);
		LinkedContentAnalyticData analyticData = _analyticDataFactory.Create(CurrencyAmplitudeAnalytic.SourceType.MiniEventRating);
		return _linkedContentFactory.Create(rewType, selector, 1, 0, ContentType.Main, analyticData) as LootboxLinkedContent;
	}
}
