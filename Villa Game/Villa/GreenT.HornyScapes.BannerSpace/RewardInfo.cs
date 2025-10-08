using StripClub.Model;
using StripClub.Model.Cards;

namespace GreenT.HornyScapes.BannerSpace;

public class RewardInfo
{
	public readonly LinkedContent LinkedContent;

	public readonly int Chance;

	public readonly bool IsNew;

	public readonly bool IsMain;

	public readonly Rarity Rarity;

	public RewardInfo(Rarity rarity, LinkedContent linkedContent, int chance, bool isNew, bool isMain)
	{
		Rarity = rarity;
		LinkedContent = linkedContent;
		Chance = chance;
		IsNew = isNew;
		IsMain = isMain;
	}
}
