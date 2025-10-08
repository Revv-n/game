using System;
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
		compositionStream = ObservableExtensions.Subscribe<double>(Observable.Select<List<double>, double>(Observable.Select<IMultiplier, List<double>>(Observable.Merge<IMultiplier>((IObservable<IMultiplier>)onAdd, new IObservable<IMultiplier>[1] { (IObservable<IMultiplier>)onRemove }), (Func<IMultiplier, List<double>>)((IMultiplier _) => multipliersList.Select((IMultiplier item) => item.Factor.Value).ToList())), (Func<List<double>, double>)Composite), (Action<double>)delegate(double result)
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
