using GreenT.Bonus;
using GreenT.Types;
using UnityEngine;

namespace StripClub.Model.Cards;

public interface ICard
{
	int GroupID { get; }

	int ID { get; }

	string NameKey { get; }

	string DescriptionKey { get; }

	ContentType ContentType { get; }

	Rarity Rarity { get; }

	IBonus Bonus { get; }

	int PromotePatternsID { get; }

	BankImages BankImages { get; }

	Sprite ProgressBarIcon { get; }
}
