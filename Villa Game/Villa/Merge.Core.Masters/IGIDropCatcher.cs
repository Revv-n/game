namespace Merge.Core.Masters;

internal interface IGIDropCatcher
{
	bool IsCatchesDrop(GameItem item);

	void CatchDrop(GameItem item);
}
