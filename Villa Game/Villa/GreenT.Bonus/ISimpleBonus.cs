using StripClub.Model;

namespace GreenT.Bonus;

public interface ISimpleBonus : ICommand
{
	string Name { get; }

	bool IsApplied { get; }
}
