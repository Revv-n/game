using GreenT.HornyScapes.Events;
using StripClub.Model;

namespace GreenT.Bonus;

public abstract class Bonus : IBonus, IAmendableBonus<double[]>, IValuableBonus<double[]>, ISimpleBonus, ICommand, ITypeBonus
{
	public abstract string Name { get; }

	public double[] Values { get; }

	public int Level { get; protected set; }

	public bool IsApplied { get; protected set; }

	public BonusType BonusType { get; protected set; }

	public Bonus(double[] values, BonusType bonusType)
	{
		Values = values;
		BonusType = bonusType;
	}

	public abstract void Apply();

	public abstract void Undo();

	public virtual void SetLevel(int level)
	{
		Level = level;
	}

	public override string ToString()
	{
		return this.ToString(Level);
	}
}
