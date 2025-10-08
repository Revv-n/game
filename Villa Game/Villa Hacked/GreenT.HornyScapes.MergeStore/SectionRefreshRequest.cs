using StripClub.Model.Data;

namespace GreenT.HornyScapes.MergeStore;

public struct SectionRefreshRequest
{
	public readonly Cost Cost;

	public readonly StoreSection Section;

	private SectionRefreshRequest(int price, StoreSection section)
	{
		Cost = new Cost(price, section.CurrencyType);
		Section = section;
	}

	public static SectionRefreshRequest GetFree(StoreSection section)
	{
		return new SectionRefreshRequest(0, section);
	}

	public static SectionRefreshRequest Get(StoreSection section)
	{
		return new SectionRefreshRequest(section.RefreshPrice, section);
	}
}
