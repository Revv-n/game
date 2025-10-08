using System.Collections.Generic;
using System.Linq;

namespace GreenT.Multiplier;

public class MultiplyingCompositeMultiplier : CompositeMultiplier
{
	public MultiplyingCompositeMultiplier(double defaultValue = 1.0)
		: base(defaultValue)
	{
	}

	public MultiplyingCompositeMultiplier()
	{
	}

	protected override double Composite(IList<double> multipliersList)
	{
		return multipliersList.Aggregate((double x, double y) => x * y);
	}
}
