using GreenT.HornyScapes.Data;

namespace GreenT.HornyScapes.Monetization;

[Mapper]
public class Product
{
	private string lot_id;

	private int item_id;

	private string price;

	public string LotID => lot_id;

	public int ItemId => item_id;

	public string Price => price;

	public Product(string lot_id, int item_id, string price)
	{
		this.lot_id = lot_id;
		this.item_id = item_id;
		this.price = price;
	}
}
