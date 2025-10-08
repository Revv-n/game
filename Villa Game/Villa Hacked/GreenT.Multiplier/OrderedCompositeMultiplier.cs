using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;

namespace GreenT.Multiplier;

public class OrderedCompositeMultiplier : IOrderedCompositeMultiplier, IMultiplier
{
	protected ReactiveProperty<double> factor;

	private double defaultValue;

	private Dictionary<IMultiplier, int> multipliersList = new Dictionary<IMultiplier, int>();

	private Dictionary<IMultiplier, IDisposable> subscribesToMultipliers = new Dictionary<IMultiplier, IDisposable>();

	private Func<IDictionary<IMultiplier, int>, double, double> composite;

	public IReadOnlyReactiveProperty<double> Factor => (IReadOnlyReactiveProperty<double>)(object)ReactivePropertyExtensions.ToReadOnlyReactiveProperty<double>((IObservable<double>)factor);

	public OrderedCompositeMultiplier(Func<IDictionary<IMultiplier, int>, double, double> compositeFunction, double defaultValue = 1.0)
	{
		this.defaultValue = defaultValue;
		factor = new ReactiveProperty<double>(defaultValue);
		composite = compositeFunction;
	}

	public void Add(IMultiplier multiplier, int order)
	{
		if (multipliersList.ContainsKey(multiplier))
		{
			multipliersList[multiplier] = order;
			return;
		}
		multipliersList.Add(multiplier, order);
		IDisposable value = ObservableExtensions.Subscribe<double>((IObservable<double>)multiplier.Factor, (Action<double>)delegate
		{
			factor.Value = composite(multipliersList, defaultValue);
		});
		subscribesToMultipliers.Add(multiplier, value);
	}

	public bool Contains(IMultiplier multiplier)
	{
		return multipliersList.ContainsKey(multiplier);
	}

	public void Remove(IMultiplier multiplier)
	{
		if (multipliersList.Remove(multiplier))
		{
			subscribesToMultipliers[multiplier].Dispose();
			subscribesToMultipliers.Remove(multiplier);
			factor.Value = composite(multipliersList, defaultValue);
		}
	}

	public static double DefaultComposite(IDictionary<IMultiplier, int> simpleMultipliersDictionary, double defaultValue = 1.0)
	{
		IOrderedEnumerable<IGrouping<int, KeyValuePair<IMultiplier, int>>> orderedEnumerable = from row in simpleMultipliersDictionary
			group row by row.Value into @group
			orderby @group.Key
			select @group;
		double num = defaultValue;
		foreach (IGrouping<int, KeyValuePair<IMultiplier, int>> item in orderedEnumerable)
		{
			double num2 = 1.0;
			foreach (KeyValuePair<IMultiplier, int> item2 in item)
			{
				num2 += item2.Key.Factor.Value;
			}
			if (num2 != 1.0)
			{
				num *= num2;
			}
		}
		return num;
	}
}
