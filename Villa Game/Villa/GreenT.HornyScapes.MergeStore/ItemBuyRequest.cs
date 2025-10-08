namespace GreenT.HornyScapes.MergeStore;

public class ItemBuyRequest
{
	public readonly Item Item;

	public readonly ButtonPosition ButtonPosition;

	public StoreSection Section;

	public ItemBuyRequest(Item item, ButtonPosition buttonPosition)
	{
		Item = item;
		ButtonPosition = buttonPosition;
	}

	public ItemBuyRequest AddSection(StoreSection section)
	{
		Section = section;
		return this;
	}
}
