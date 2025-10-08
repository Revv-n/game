using System;

namespace Merge;

public interface IBlockModulesAction
{
	FilterNode<GIModuleType> ActionsFilter { get; }

	event Action OnBlockActionChange;
}
