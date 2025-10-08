using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.Data;
using GreenT.HornyScapes.Bank.Data;
using GreenT.HornyScapes.Lockers;
using StripClub.Model;
using StripClub.Model.Shop;
using Zenject;

namespace GreenT.HornyScapes.Bank;

public class OfferBaseFactory
{
	protected readonly IFactory<UnlockType, string, LockerSourceType, ILocker> lockerFactory;

	protected readonly LotManager lotManager;

	protected readonly ISaver saver;

	protected readonly IClock clock;

	public OfferBaseFactory(IFactory<UnlockType, string, LockerSourceType, ILocker> lockerFactory, LotManager lotManager, ISaver saver, IClock clock)
	{
		this.lockerFactory = lockerFactory;
		this.lotManager = lotManager;
		this.saver = saver;
		this.clock = clock;
	}

	protected CompositeLocker CreateCompositeLocker(OfferMapper mapper, BundleLot[] offerBundles)
	{
		List<ILocker> list = new List<ILocker>();
		for (int i = 0; i != mapper.unlock_type.Length; i++)
		{
			ILocker item = lockerFactory.Create(mapper.unlock_type[i], mapper.unlock_value[i], LockerSourceType.None);
			list.Add(item);
		}
		for (int j = 0; j != offerBundles.Length; j++)
		{
			ILocker locker = offerBundles[j].Locker;
			list.Add(locker);
		}
		return new CompositeLocker(list);
	}

	protected virtual void CreateBaseStructures(OfferMapper mapper, out BundleLot[] offerBundles, out CompositeLocker locker, out TimeLocker timeLocker, out CompositeLocker lockerWithTimer)
	{
		offerBundles = GetBundles(mapper.bundles).ToArray();
		locker = CreateCompositeLocker(mapper, offerBundles);
		timeLocker = new TimeLocker();
		lockerWithTimer = new CompositeLocker(new ILocker[2] { locker, timeLocker });
	}

	protected List<BundleLot> GetBundles(int[] bundleIDs)
	{
		BundleLot[] source = lotManager.GetLot<BundleLot>().ToArray();
		List<BundleLot> list = new List<BundleLot>();
		for (int i = 0; i < bundleIDs.Length; i++)
		{
			int bundle_id = bundleIDs[i];
			try
			{
				BundleLot item = source.First((BundleLot _bundle) => _bundle.ID == bundle_id);
				list.Add(item);
			}
			catch (Exception innerException)
			{
				throw innerException.SendException("В коллекции лотов нет бандла с ID: " + bundle_id);
			}
		}
		return list;
	}
}
