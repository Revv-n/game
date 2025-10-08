using GreenT.HornyScapes.MergeCore;

namespace Merge.Core.Masters;

public interface ICreateItemListener
{
	int Priority { get; }

	void AtItemCreated(GameItem gi, MergeField mergeField);
}
