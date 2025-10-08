using System.Collections.Generic;
using System.Linq;
using UniRx;

namespace GreenT.Multiplier;

public class BoosterSummingMultiplier : CompositeMultiplier
{
	public BoosterSummingMultiplier(double defaultValue = 0.0)
		: base(defaultValue)
	{
	}

	public BoosterSummingMultiplier()
		: base(0.0)
	{
	}

	protected sealed override void ObserveMultipliers()
	{
		if (compositionStream != null)
		{
			return;
		}
		factor.Value = Composite(multipliersList.Select((IMultiplier item) => item.Factor.Value).ToList());
		compositionStream = (from _ in onAdd.Merge(onRemove)
			select multipliersList.Select((IMultiplier item) => item.Factor.Value).ToList()).Select(Composite).Subscribe(delegate(double result)
		{
			factor.Value = result;
		});
	}

	protected override double Composite(IList<double> multipliersList)
	{
		if (!multipliersList.Any())
		{
			return 0.0;
		}
		return multipliersList.Aggregate((double x, double y) => x + y);
	}
}
