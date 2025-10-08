using StripClub.Model;

namespace GreenT.Bonus;

public interface ITypeBonus : ISimpleBonus, ICommand
{
	BonusType BonusType { get; }
}
public interface ITypeBonus<TType, TValue> : IValuableBonus<TValue>, ISimpleBonus, ICommand
{
	TType BonusType { get; }
}
