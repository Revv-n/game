using GreenT.HornyScapes.Data;
using StripClub.Model.Cards;

namespace StripClub.Model.Data;

[Mapper]
public class PromotePatternMapper
{
	public int id;

	public Rarity rarity;

	public int current_level;

	public int promote_cards_value;

	public int promote_resource_value;
}
