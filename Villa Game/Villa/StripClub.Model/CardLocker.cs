using System.Collections.Generic;
using System.Linq;
using StripClub.Model.Cards;

namespace StripClub.Model;

public class CardLocker<T> : Locker where T : ICard
{
	public int CardID { get; }

	public Attitude CardState { get; }

	public CardLocker(int cardID, Attitude attitude)
	{
		CardID = cardID;
		CardState = attitude;
	}

	private bool IsTargetCard(T ownedCard)
	{
		return ownedCard.ID == CardID;
	}

	public void Set(T ownedCard)
	{
		if (IsTargetCard(ownedCard))
		{
			isOpen.Value = CardState == Attitude.Owned;
		}
	}

	public void Set(IEnumerable<T> ownedCards)
	{
		bool flag = ownedCards.Any((T _card) => IsTargetCard(_card));
		isOpen.Value = ((CardState == Attitude.Owned) ? flag : (!flag));
	}
}
