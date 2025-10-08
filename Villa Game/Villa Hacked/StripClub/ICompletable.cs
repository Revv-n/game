using System;

namespace StripClub;

public interface ICompletable<T>
{
	T Progress { get; }

	T Target { get; }

	IObservable<T> OnProgressUpdate { get; }

	bool IsComplete();
}
