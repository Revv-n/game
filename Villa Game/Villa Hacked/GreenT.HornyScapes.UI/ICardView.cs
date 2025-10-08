using StripClub.Model.Cards;

namespace GreenT.HornyScapes.UI;

public interface ICardView
{
	ICard Card { get; }

	void Set(ICard card);
}
