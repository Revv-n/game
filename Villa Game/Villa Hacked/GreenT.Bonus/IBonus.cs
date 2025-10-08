using StripClub.Model;

namespace GreenT.Bonus;

public interface IBonus : IAmendableBonus<double[]>, IValuableBonus<double[]>, ISimpleBonus, ICommand, ITypeBonus
{
}
public interface IBonus<TValue> : IValuableBonus<TValue>, ISimpleBonus, ICommand, ITypeBonus
{
}
public interface IBonus<TType, TValue> : IAmendableBonus<TType, TValue>, ITypeBonus<TType, TValue>, IValuableBonus<TValue>, ISimpleBonus, ICommand
{
}
