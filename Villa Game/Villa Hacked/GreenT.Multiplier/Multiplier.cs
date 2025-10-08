using System;
using UniRx;

namespace GreenT.Multiplier;

public class Multiplier : IMultiplier
{
	protected IReactiveProperty<double> factor;

	public IReadOnlyReactiveProperty<double> Factor => (IReadOnlyReactiveProperty<double>)(object)ReactivePropertyExtensions.ToReadOnlyReactiveProperty<double>((IObservable<double>)factor);

	public Multiplier(double value)
	{
		factor = (IReactiveProperty<double>)(object)new ReactiveProperty<double>(value);
	}

	public virtual void Set(double value)
	{
		factor.Value = value;
	}
}
