using System;

namespace StripClub.Model;

public class PlayerItemsEventArgs : EventArgs
{
	public Item Item { get; private set; }

	public PlayerItemsEventArgs(Item updatedItem)
	{
		Item = updatedItem;
	}
}
