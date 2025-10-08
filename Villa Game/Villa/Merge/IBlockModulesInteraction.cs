using System;

namespace Merge;

public interface IBlockModulesInteraction
{
	FilterNode<GIModuleType> InteractionsFilter { get; }

	event Action OnBlockInteractionChange;
}
