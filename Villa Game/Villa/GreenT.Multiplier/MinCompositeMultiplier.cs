using System.Collections.Generic;
using System.Linq;
using UniRx;

namespace GreenT.Multiplier;

public class MinCompositeMultiplier : CompositeMultiplier
{
	public MinCompositeMultiplier(double defaultValue = 0.0)
		: base(defaultValue)
	{
	}

	public MinCompositeMultiplier()
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
		return multipliersList.OrderBy((double value) => value).FirstOrDefault();
	}

	public MinCompositeMultiplier GetByValue(int value)
	{
		return multipliersList.FirstOrDefault((IMultiplier item) => (int)item.Factor.Value == value) as MinCompositeMultiplier;
	}
}
