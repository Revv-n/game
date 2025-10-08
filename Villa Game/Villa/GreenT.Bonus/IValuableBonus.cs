using StripClub.Model;

namespace GreenT.Bonus;

public interface IValuableBonus<TValue> : ISimpleBonus, ICommand
{
	TValue Values { get; }
}
