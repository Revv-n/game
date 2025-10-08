using GreenT.Types;

namespace GreenT.HornyScapes.MiniEvents;

public interface IContentable
{
	bool HasAnyContentAvailable(CompositeIdentificator identificator);
}
