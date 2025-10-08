using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.Sellouts.Services;
using Zenject;

namespace GreenT.HornyScapes.Sellouts.Factories;

public class SelloutRewardsConditionFactory : IFactory<int, int, IConditionReceivingReward[]>, IFactory
{
	private SelloutStateManager _selloutStateManager;

	[Inject]
	public SelloutRewardsConditionFactory(SelloutStateManager selloutStateManager)
	{
		_selloutStateManager = selloutStateManager;
	}

	public IConditionReceivingReward[] Create(int selloutId, int pointsRequirement)
	{
		return new IConditionReceivingReward[1]
		{
			new SelloutConditionsReceivingPoints(_selloutStateManager, selloutId, pointsRequirement)
		};
	}
}
