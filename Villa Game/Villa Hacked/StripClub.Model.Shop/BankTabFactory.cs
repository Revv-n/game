using System;
using GreenT.HornyScapes;
using GreenT.HornyScapes.Bank.BankTabs;
using GreenT.HornyScapes.Lockers;
using Zenject;

namespace StripClub.Model.Shop;

public class BankTabFactory : IFactory<BankTabMapper, BankTab>, IFactory
{
	private readonly LockerFactory lockerFactory;

	private readonly BundlesProviderBase _bundlesProvider;

	public BankTabFactory(LockerFactory lockerFactory, BundlesProviderBase bundlesProvider)
	{
		this.lockerFactory = lockerFactory;
		_bundlesProvider = bundlesProvider;
	}

	public BankTab Create(BankTabMapper mapper)
	{
		try
		{
			ILocker locker = CreateLocker(mapper.unlock_type, mapper.unlock_value);
			LayoutSettings layoutParams = null;
			if (!string.IsNullOrEmpty(mapper.parameters))
			{
				if (mapper.layout_type != LayoutType.SlotsWithBanner)
				{
					throw new NotImplementedException("Couldn't find parameters translator for this layout");
				}
				layoutParams = new SlotsAndBannerSettings(mapper.parameters);
			}
			return new BankTab(mapper.id, mapper.position, mapper.layout_type, _bundlesProvider, mapper.content_source, mapper.icon, locker, layoutParams);
		}
		catch (Exception innerException)
		{
			throw new Exception("Can't parse BankTab with id:" + mapper.id, innerException);
		}
	}

	private ILocker CreateLocker(UnlockType[] unlock_type, string[] unlock_value)
	{
		if (unlock_type == null || unlock_type.Length == 0)
		{
			return new PermanentLocker(isOpen: true);
		}
		ILocker[] array = new ILocker[unlock_type.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = lockerFactory.Create(unlock_type[i], unlock_value[i], LockerSourceType.BankTab);
		}
		return new CompositeLocker(array);
	}
}
