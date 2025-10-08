using System;
using System.Collections.Generic;

namespace Merge;

public class BlockMergeNode
{
	public List<GIKey> keysList;

	public Action<GameItem, GameItem> callback;

	public bool allowIfAtLeastOneLeft = true;

	public BlockMergeNode(List<GIKey> keysList, Action<GameItem, GameItem> callback)
	{
		this.keysList = keysList;
		this.callback = callback;
	}
}
