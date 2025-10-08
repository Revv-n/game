using GreenT.HornyScapes.Tasks;
using StripClub.Model.Shop;

namespace GreenT.HornyScapes.MergeStore;

public class ItemFactory
{
	private readonly RegularItemFactory _regularFactory;

	private readonly PremiumItemFactory _premiumFactory;

	private readonly ItemFactoryHelper _itemFactoryHelper;

	public ItemFactory(MergeItemInfoService mergeItemInfoService, TaskManagerCluster taskManagerCluster, ICurrencyProcessor currencyProcessor)
	{
		_itemFactoryHelper = new ItemFactoryHelper(mergeItemInfoService);
		_regularFactory = new RegularItemFactory(mergeItemInfoService, taskManagerCluster, currencyProcessor, _itemFactoryHelper);
		_premiumFactory = new PremiumItemFactory(_itemFactoryHelper, mergeItemInfoService);
	}

	public Item[] GenerateItemsForSection(StoreSection section, string bundle)
	{
		int discountSlotsCount = _itemFactoryHelper.DetermineDiscountSlotsCount(section.DiscountChances);
		Item[] array = ((section.Type == SectionType.Premium) ? _premiumFactory.Generate(section, bundle) : _regularFactory.Generate(section, bundle));
		_itemFactoryHelper.ShuffleItems(array);
		_itemFactoryHelper.ApplyDiscountsToItems(array, discountSlotsCount, section);
		return array;
	}
}
