using System.Collections.Generic;
using System.Linq;

namespace GreenT.Multiplier;

public class SummingCompositeMultiplier : CompositeMultiplier
{
	public SummingCompositeMultiplier(double defaultValue = 0.0)
		: base(defaultValue)
	{
	}

	public SummingCompositeMultiplier()
		: base(0.0)
	{
	}

	protected override double Composite(IList<double> multipliersList)
	{
		return multipliersList.Aggregate((double x, double y) => x + y);
	}
}
