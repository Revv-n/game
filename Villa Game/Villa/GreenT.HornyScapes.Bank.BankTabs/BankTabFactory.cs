using System;
using StripClub.Model;
using StripClub.Model.Shop;
using Zenject;

namespace GreenT.HornyScapes.Bank.BankTabs;

public class BankTabFactory : IFactory<BankTabMapper, BankTab>, IFactory
{
	private readonly LockerFactory lockerFactory;

	private readonly BundlesProviderBase bundlesProvider;

	public BankTabFactory(LockerFactory lockerFactory, BundlesProviderBase bundlesProvider)
	{
		this.lockerFactory = lockerFactory;
		this.bundlesProvider = bundlesProvider;
	}

	public BankTab Create(BankTabMapper mapper)
	{
		CompositeLocker locker = LockerFactory.CreateFromParamsArray(mapper.unlock_type, mapper.unlock_value, lockerFactory, LockerSourceType.BankTab);
		LayoutSettings layoutParams = null;
		if (!string.IsNullOrEmpty(mapper.parameters))
		{
			if (mapper.layout_type != LayoutType.SlotsWithBanner)
			{
				throw new NotImplementedException("Couldn't find parameters translator for this layout");
			}
			layoutParams = new SlotsAndBannerSettings(mapper.parameters);
		}
		return new BankTab(mapper.id, mapper.position, mapper.layout_type, bundlesProvider, mapper.content_source, mapper.icon, locker, layoutParams);
	}
}
