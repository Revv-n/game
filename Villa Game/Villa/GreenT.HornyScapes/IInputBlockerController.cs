using System;

namespace GreenT.HornyScapes;

public interface IInputBlockerController
{
	IObservable<IInputBlocker> OnUpdate { get; }

	bool ClickBlock { get; }

	void AddBlocker(IInputBlocker inputBlocker);

	void Remove(IInputBlocker inputBlocker);
}
