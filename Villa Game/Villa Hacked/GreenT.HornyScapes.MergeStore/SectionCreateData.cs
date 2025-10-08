using GreenT.Types;
using StripClub.Model;

namespace GreenT.HornyScapes.MergeStore;

public class SectionCreateData
{
	public readonly int ID;

	public readonly ContentType ContentType;

	public readonly SectionType SectionType;

	public readonly int RefreshTime;

	public readonly int RefreshCost;

	public readonly CurrencyType RefreshCurrency;

	public readonly int[] DiscountChances;

	public readonly int ItemsCount;

	public readonly int EnergyThreshold;

	public readonly int LowEnergyLowerTierChance;

	public readonly int SaleTierDifference;

	public readonly int[] RarityChance;

	public SectionCreateData(int id, SectionType sectionType, int refreshTime, int refreshCost, CurrencyType refreshCurrency, int[] discountChances, int itemsCount, ContentType contentType, int energyThreshold, int lowEnergyLowerTierChance, int saleTierDifference, int[] rarityChance)
	{
		ID = id;
		RarityChance = rarityChance;
		SectionType = sectionType;
		RefreshTime = refreshTime;
		RefreshCost = refreshCost;
		RefreshCurrency = refreshCurrency;
		DiscountChances = discountChances;
		ItemsCount = itemsCount;
		ContentType = contentType;
		EnergyThreshold = energyThreshold;
		LowEnergyLowerTierChance = lowEnergyLowerTierChance;
		SaleTierDifference = saleTierDifference;
	}
}
