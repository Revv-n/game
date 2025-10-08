namespace Merge.Core.Masters;

public interface IRemoveItemListener
{
	int Priority { get; }

	void AtItemRemoved(GameItem gi);
}
