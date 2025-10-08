using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.Relationships.Views;
using GreenT.Types;
using StripClub.Model;
using StripClub.Model.Shop;
using Zenject;

namespace GreenT.HornyScapes.Relationships.Factories;

public class RelationshipRewardConditionFactory : IFactory<int, int, IConditionReceivingReward[]>, IFactory
{
	private readonly ICurrencyProcessor _currencyProcessor;

	public RelationshipRewardConditionFactory(ICurrencyProcessor currencyProcessor)
	{
		_currencyProcessor = currencyProcessor;
	}

	public IConditionReceivingReward[] Create(int relationshipId, int pointsRequirement)
	{
		IConditionReceivingReward[] array = new IConditionReceivingReward[1];
		CompositeIdentificator currencyIdentificator = new CompositeIdentificator(relationshipId);
		array[0] = new RelationshipConditionsReceivingPoints(_currencyProcessor, CurrencyType.LovePoints, pointsRequirement, currencyIdentificator);
		return array;
	}
}
