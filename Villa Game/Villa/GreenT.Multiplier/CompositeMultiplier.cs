using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;

namespace GreenT.Multiplier;

public abstract class CompositeMultiplier : ICompositeMultiplier, IMultiplier, IDisposable
{
	protected ReactiveProperty<double> factor;

	protected readonly List<IMultiplier> multipliersList = new List<IMultiplier>();

	protected Subject<IMultiplier> onAdd = new Subject<IMultiplier>();

	protected Subject<IMultiplier> onRemove = new Subject<IMultiplier>();

	protected IDisposable compositionStream;

	public IReadOnlyReactiveProperty<double> Factor => factor;

	public CompositeMultiplier(double defaultValue = 1.0)
	{
		factor = new ReactiveProperty<double>(defaultValue);
	}

	public void Add(IMultiplier multiplier)
	{
		if (!Contains(multiplier))
		{
			multipliersList.Add(multiplier);
			onAdd.OnNext(multiplier);
			ObserveMultipliers();
		}
	}

	protected virtual void ObserveMultipliers()
	{
		compositionStream?.Dispose();
		compositionStream = multipliersList.Select((IMultiplier _multiplier) => _multiplier.Factor).CombineLatest().Select(Composite)
			.Subscribe(delegate(double result)
			{
				factor.Value = result;
			});
	}

	protected abstract double Composite(IList<double> multipliersList);

	public bool Contains(IMultiplier multiplier)
	{
		return multipliersList.Contains(multiplier);
	}

	public void Remove(IMultiplier multiplier)
	{
		if (multipliersList.Remove(multiplier))
		{
			onRemove.OnNext(multiplier);
			ObserveMultipliers();
		}
	}

	public void Dispose()
	{
		compositionStream?.Dispose();
	}
}
