using System.Linq;
using GreenT.Types;
using StripClub.Model;
using Zenject;

namespace GreenT.HornyScapes.MergeStore;

public class CreateDataFactory : IFactory<MergeStoreMapper, CreateData>, IFactory
{
	private const int RegularSectionItemsCount = 6;

	private const int PremiumSectionItemsCount = 2;

	private readonly LockerFactory _lockerFactory;

	public CreateDataFactory(LockerFactory lockerFactory)
	{
		_lockerFactory = lockerFactory;
	}

	public CreateData Create(MergeStoreMapper mapper)
	{
		CompositeLocker locker = CreateLocker(mapper);
		SectionCreateData regularSectionCreateData = CreateRegularSection(mapper);
		SectionCreateData premiumSectionCreateData = CreatePremiumSection(mapper);
		return new CreateData(mapper.id, mapper.target_id, locker, regularSectionCreateData, premiumSectionCreateData, mapper.use_unique_premium);
	}

	private SectionCreateData CreateRegularSection(MergeStoreMapper mapper)
	{
		ContentType contentType = ((mapper.Type == ConfigContentType.Event) ? ContentType.Event : ContentType.Main);
		return new SectionCreateData(mapper.id, SectionType.Regular, mapper.regular_section_refresh_time, mapper.regular_section_refresh_cost, mapper.regular_section_refresh_currency, mapper.regular_section_discount_chances, 6, contentType, mapper.energy_threshold, mapper.low_energy_lower_tier_chance, mapper.sale_tier_difference, mapper.discount_rarity_chance);
	}

	private SectionCreateData CreatePremiumSection(MergeStoreMapper mapper)
	{
		if (!mapper.use_unique_premium)
		{
			return null;
		}
		ContentType contentType = ((mapper.Type == ConfigContentType.Event) ? ContentType.Event : ContentType.Main);
		return new SectionCreateData(mapper.id, SectionType.Premium, mapper.premium_section_refresh_time, mapper.premium_section_refresh_cost, mapper.premium_section_refresh_currency, mapper.premium_section_discount_chances, 2, contentType, mapper.energy_threshold, mapper.low_energy_lower_tier_chance, mapper.sale_tier_difference, mapper.discount_rarity_chance);
	}

	private CompositeLocker CreateLocker(MergeStoreMapper mapper)
	{
		return new CompositeLocker(mapper.unlock_type.Select((UnlockType t, int i) => _lockerFactory.Create(t, mapper.unlock_value[i], LockerSourceType.BundleLot)).ToList());
	}
}
