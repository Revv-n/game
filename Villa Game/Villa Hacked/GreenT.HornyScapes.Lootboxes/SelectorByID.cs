namespace GreenT.HornyScapes.Lootboxes;

public class SelectorByID : Selector
{
	public int ID { get; }

	public SelectorByID(int id)
	{
		ID = id;
	}
}
