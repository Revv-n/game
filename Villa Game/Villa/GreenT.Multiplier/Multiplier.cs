using UniRx;

namespace GreenT.Multiplier;

public class Multiplier : IMultiplier
{
	protected IReactiveProperty<double> factor;

	public IReadOnlyReactiveProperty<double> Factor => factor.ToReadOnlyReactiveProperty();

	public Multiplier(double value)
	{
		factor = new ReactiveProperty<double>(value);
	}

	public virtual void Set(double value)
	{
		factor.Value = value;
	}
}
