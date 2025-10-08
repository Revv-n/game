using StripClub.Model.Cards;

namespace GreenT.HornyScapes.Lootboxes;

public class CardSelector : Selector
{
	public enum TargetPool
	{
		In,
		Out
	}

	public Rarity Rarity { get; }

	public TargetPool Pool { get; }

	public CardSelector(Rarity rarity, TargetPool pool)
	{
		Rarity = rarity;
		Pool = pool;
	}
}
