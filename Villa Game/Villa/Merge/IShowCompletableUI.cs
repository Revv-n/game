using System;

namespace Merge;

public interface IShowCompletableUI
{
	bool FullVisible { get; }

	event Action OnShowComplete;
}
