using GreenT.Multiplier;

namespace GreenT.Bonus;

public abstract class MultiplierBonus : Bonus
{
	protected readonly IMultiplierTotalContainer<int> multipliersContainer;

	public override string Name { get; }

	public abstract IMultiplier Multiplier { get; }

	protected MultiplierBonus(IMultiplierTotalContainer<int> multipliersContainer, double[] values, string name, BonusType bonusType)
		: base(values, bonusType)
	{
		this.multipliersContainer = multipliersContainer;
		Name = name;
	}
}
