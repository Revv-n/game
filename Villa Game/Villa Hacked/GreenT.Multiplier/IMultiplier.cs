using UniRx;

namespace GreenT.Multiplier;

public interface IMultiplier
{
	IReadOnlyReactiveProperty<double> Factor { get; }
}
