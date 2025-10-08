using System.Collections;
using System.Collections.Generic;
using UniRx;

namespace GreenT.Model.Reactive;

public interface IReactiveCollection<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable, IReadOnlyReactiveCollection<T>, IReadOnlyReactiveCollection<T>
{
	new int Count { get; }

	new T this[int index] { get; set; }

	void Move(int oldIndex, int newIndex);
}
