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

public class OfferTabFactory : IFactory<OfferMapper, OfferSettings>, IFactory
{
	private readonly BundlesProviderBase bundlesProvider;

	private readonly TimeInstaller.TimerCollection timerCollection;

	private readonly LockerFactory lockerFactory;

	private readonly LotManager lotManager;

	private readonly ISaver saver;

	private readonly IClock clock;

	public OfferTabFactory(LockerFactory lockerFactory, LotManager lotManager, ISaver saver, IClock clock, BundlesProviderBase bundlesProvider, TimeInstaller.TimerCollection timerCollection)
	{
		this.lockerFactory = lockerFactory;
		this.lotManager = lotManager;
		this.saver = saver;
		this.clock = clock;
		this.bundlesProvider = bundlesProvider;
		this.timerCollection = timerCollection;
	}

	public OfferSettings Create(OfferMapper mapper)
	{
		try
		{
			OfferSettings.ViewSettings layoutParams = new OfferSettings.ViewSettings(bundlesProvider, mapper.content_source, mapper.view_parameters);
			BundleLot[] offerBundles = GetBundles(mapper.bundles).ToArray();
			CompositeLocker compositeLocker = CreateCompositeLocker(mapper, offerBundles);
			TimeLocker timeLocker = new TimeLocker();
			CompositeLocker lockerWithTimer = new CompositeLocker(new ILocker[2] { compositeLocker, timeLocker });
			OfferSettings offerSettings = new OfferSettings(mapper, offerBundles, compositeLocker, lockerWithTimer, layoutParams, timeLocker, clock, timerCollection);
			saver.Add(offerSettings);
			return offerSettings;
		}
		catch (Exception innerException)
		{
			throw innerException.SendException("Ошибка при парсинге настроек оффера с id: " + mapper.id);
		}
	}

	private List<BundleLot> GetBundles(int[] bundleIDs)
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

	protected CompositeLocker CreateCompositeLocker(OfferMapper mapper, BundleLot[] offerBundles)
	{
		List<ILocker> list = new List<ILocker>();
		for (int i = 0; i != mapper.unlock_type.Length; i++)
		{
			ILocker item = lockerFactory.Create(mapper.unlock_type[i], mapper.unlock_value[i], LockerSourceType.OfferTab);
			list.Add(item);
		}
		for (int j = 0; j != offerBundles.Length; j++)
		{
			ILocker locker = offerBundles[j].Locker;
			list.Add(locker);
		}
		return new CompositeLocker(list);
	}
}
