using GreenT.Types;

namespace GreenT.HornyScapes;

public interface IViewController
{
	void Show(CompositeIdentificator identificator, bool isMultiTabbed);

	void HideAll();
}
