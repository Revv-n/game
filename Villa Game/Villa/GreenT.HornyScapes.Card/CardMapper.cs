using System;
using GreenT.Bonus;
using StripClub.Model;
using StripClub.Model.Cards;

namespace GreenT.HornyScapes.Card;

[Serializable]
public abstract class CardMapper
{
	public int id;

	public BonusType bonus;

	public double[] bonus_value;

	public bool affect_all;

	public Rarity rarity;

	public UnlockType[] unlock_type;

	public string[] unlock_value;
}
