namespace StripClub.Model.Cards;

public class PromotePattern
{
	public int promote_cards_value;

	public int promote_resource_value;

	public PromotePattern(int promote_cards_value, int promote_resource_value)
	{
		this.promote_cards_value = promote_cards_value;
		this.promote_resource_value = promote_resource_value;
	}
}
