using UniRx;

namespace GreenT.HornyScapes.Data;

public interface IInitializerState
{
	IReadOnlyReactiveProperty<bool> IsInitialized { get; }

	IReadOnlyReactiveProperty<bool> IsRequiredInitialized { get; }
}
