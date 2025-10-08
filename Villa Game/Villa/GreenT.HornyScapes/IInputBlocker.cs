using System;

namespace GreenT.HornyScapes;

public interface IInputBlocker
{
	IObservable<IInputBlocker> OnUpdate { get; }

	bool IsLaunched { get; }
}
