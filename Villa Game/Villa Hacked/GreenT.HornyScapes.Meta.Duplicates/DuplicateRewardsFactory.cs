using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.Lootboxes;
using Zenject;

namespace GreenT.HornyScapes.Meta.Duplicates;

public class DuplicateRewardsFactory : IFactory<DuplicateRewardMapper, DuplicateReward>, IFactory
{
	private readonly LinkedContentFactory _contentFactory;

	private readonly LinkedContentAnalyticDataFactory _analyticDataFactory;

	public DuplicateRewardsFactory(LinkedContentFactory contentFactory, LinkedContentAnalyticDataFactory analyticFactory)
	{
		_contentFactory = contentFactory;
		_analyticDataFactory = analyticFactory;
	}

	public DuplicateReward Create(DuplicateRewardMapper mapper)
	{
		return new DuplicateReward(mapper.id, mapper.rew_id, mapper.rew_type, CreateSelectors(mapper), mapper.rew_qty, _contentFactory, _analyticDataFactory);
	}

	private Selector[] CreateSelectors(DuplicateRewardMapper mapper)
	{
		Selector[] array = new Selector[mapper.rew_id.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = SelectorTools.CreateSelector(mapper.rew_id[i]);
		}
		return array;
	}
}
