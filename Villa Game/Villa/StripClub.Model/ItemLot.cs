namespace StripClub.Model;

public struct ItemLot
{
	public IItemInfo info;

	public int count;

	public ItemLot(IItemInfo info, int count)
	{
		this.info = info;
		this.count = count;
	}
}
