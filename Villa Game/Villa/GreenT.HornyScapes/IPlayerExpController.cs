using System;
using UniRx;

namespace GreenT.HornyScapes;

public interface IPlayerExpController
{
	IReadOnlyReactiveProperty<int> Level { get; }

	ReactiveProperty<int> Target { get; set; }

	IObservable<int> OnProgressUpdate { get; }

	int XPCount { get; }

	bool IsComplete();
}
