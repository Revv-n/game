using System;
using StripClub.Model.Cards;

namespace StripClub.Model;

public class PromoteLocker<T> : Locker where T : ICard
{
	public int CardID { get; }

	public int Level { get; }

	public Restriction Restriction { get; }

	public PromoteLocker(int cardID, int level, Restriction restriction)
	{
		CardID = cardID;
		Level = level;
		Restriction = restriction;
	}

	public void Set(T card, int promoteLevel)
	{
		if (card.ID == CardID)
		{
			switch (Restriction)
			{
			case Restriction.Min:
				isOpen.Value = promoteLevel >= Level;
				break;
			case Restriction.Max:
				isOpen.Value = promoteLevel < Level;
				break;
			case Restriction.Equal:
				isOpen.Value = promoteLevel == Level;
				break;
			default:
				throw new ArgumentOutOfRangeException("No behaviour for this type: " + Restriction);
			}
		}
	}
}
