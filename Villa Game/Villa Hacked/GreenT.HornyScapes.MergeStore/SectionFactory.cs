using GreenT.Data;
using StripClub.UI;

namespace GreenT.HornyScapes.MergeStore;

public class SectionFactory
{
	private readonly IClock _clock;

	private readonly ISaver _saver;

	private readonly SectionRefreshService _sectionRefreshService;

	public SectionFactory(IClock clock, ISaver saver, SectionRefreshService sectionRefreshService)
	{
		_clock = clock;
		_saver = saver;
		_sectionRefreshService = sectionRefreshService;
	}

	public StoreSection CreatForChet(SectionCreateData createData, string bundle)
	{
		return CreateSection(createData, bundle);
	}

	public StoreSection Create(SectionCreateData createData, string bundle)
	{
		StoreSection storeSection = CreateSection(createData, bundle);
		_saver.Add(storeSection);
		storeSection = (HaveItems(storeSection) ? _sectionRefreshService.InitializationWithOldItems(storeSection) : _sectionRefreshService.PopulateWithNewItems(storeSection, bundle));
		_sectionRefreshService.CreatStreams(bundle, storeSection);
		return storeSection;
	}

	public void FixSection(StoreSection section, string bundle)
	{
		_sectionRefreshService.PopulateWithNewItems(section, bundle);
		_sectionRefreshService.CreatStreams(bundle, section);
	}

	private static bool HaveItems(StoreSection section)
	{
		if (section.ShopItems != null)
		{
			return section.ShopItems.Count == section.ItemsCount;
		}
		return false;
	}

	private StoreSection CreateSection(SectionCreateData createData, string bundle)
	{
		GenericTimer refreshTimer = new GenericTimer();
		string saveKey = $"{bundle}_{createData.ContentType}_{createData.SectionType}";
		return new StoreSection(createData.ID, saveKey, refreshTimer, _clock, createData.ContentType, createData.SectionType, createData.RefreshTime, createData.RefreshCost, createData.RefreshCurrency, createData.DiscountChances, createData.ItemsCount, createData.EnergyThreshold, createData.LowEnergyLowerTierChance, createData.SaleTierDifference, createData.RarityChance);
	}
}
