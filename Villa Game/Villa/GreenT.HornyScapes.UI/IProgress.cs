using System;

namespace GreenT.HornyScapes.UI;

public interface IProgress<T> where T : struct, IComparable<T>, IEquatable<T>
{
	T Progress { get; }

	void Init(T relativeProgress);

	bool IsComplete();
}
