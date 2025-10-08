using GreenT.Types;
using StripClub.Model;

namespace GreenT.HornyScapes.Lootboxes;

public class UnlockTypeSelector : Selector
{
	public UnlockType UnlockType { get; }

	public CompositeIdentificator Identificator { get; }

	public UnlockTypeSelector(UnlockType unlockType, CompositeIdentificator identificator)
	{
		UnlockType = unlockType;
		Identificator = identificator;
	}
}
