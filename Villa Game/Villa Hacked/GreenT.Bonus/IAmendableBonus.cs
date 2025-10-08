using StripClub.Model;

namespace GreenT.Bonus;

public interface IAmendableBonus<TValue> : IValuableBonus<TValue>, ISimpleBonus, ICommand
{
	int Level { get; }

	void SetLevel(int level);
}
public interface IAmendableBonus<TType, TValue> : ITypeBonus<TType, TValue>, IValuableBonus<TValue>, ISimpleBonus, ICommand
{
	int Level { get; }

	void SetLevel(int level);
}
