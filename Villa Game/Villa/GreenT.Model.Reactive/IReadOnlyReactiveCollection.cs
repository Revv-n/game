using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;

namespace GreenT.Model.Reactive;

public interface IReadOnlyReactiveCollection<T> : UniRx.IReadOnlyReactiveCollection<T>, IEnumerable<T>, IEnumerable
{
	IObservable<CollectionAddERangEvent<T>> ObserveSet();
}
