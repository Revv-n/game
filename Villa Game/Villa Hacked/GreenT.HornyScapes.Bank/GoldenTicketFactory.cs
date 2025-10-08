using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.Data;
using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.Bank.Data;
using GreenT.HornyScapes.Lockers;
using StripClub.Model;
using StripClub.Model.Shop;
using Zenject;

namespace GreenT.HornyScapes.Bank;

public class GoldenTicketFactory : IFactory<GoldenTicketMapper, GoldenTicket>, IFactory
{
	private readonly LinkedContentAnalyticDataFactory analyticDataFactory;

	private readonly IFactory<UnlockType, string, LockerSourceType, ILocker> lockerFactory;

	private readonly ISaver saver;

	private readonly IClock clock;

	private readonly LotManager lotManager;

	public GoldenTicketFactory(IFactory<UnlockType, string, LockerSourceType, ILocker> lockerFactory, ISaver saver, IClock clock, LotManager lotManager, LinkedContentAnalyticDataFactory analyticDataFactory)
	{
		this.lockerFactory = lockerFactory;
		this.saver = saver;
		this.clock = clock;
		this.analyticDataFactory = analyticDataFactory;
		this.lotManager = lotManager;
	}

	public GoldenTicket Create(GoldenTicketMapper mapper)
	{
		try
		{
			BundleLot bundle = GetBundle(mapper.bundle_id);
			bundle.Content.AnalyticData.SourceType = CurrencyAmplitudeAnalytic.SourceType.GoldenTicket;
			CompositeLocker compositeLocker = CreateCompositeLocker(mapper);
			TimeLocker timeLocker = new TimeLocker();
			CompositeLocker lockerWithTimer = new CompositeLocker(new ILocker[2] { compositeLocker, timeLocker });
			GoldenTicket goldenTicket = new GoldenTicket(mapper, bundle, compositeLocker, lockerWithTimer, timeLocker, clock);
			saver.Add(goldenTicket);
			return goldenTicket;
		}
		catch (Exception innerException)
		{
			throw innerException.SendException("Error while parsing golden ticket settings with id: " + mapper.id);
		}
	}

	private BundleLot GetBundle(int bundleID)
	{
		return lotManager.GetLot<BundleLot>().ToArray().First((BundleLot _bundle) => _bundle.ID == bundleID);
	}

	protected CompositeLocker CreateCompositeLocker(GoldenTicketMapper mapper)
	{
		List<ILocker> list = new List<ILocker>();
		for (int i = 0; i != mapper.unlock_type.Length; i++)
		{
			ILocker item = lockerFactory.Create(mapper.unlock_type[i], mapper.unlock_value[i], LockerSourceType.GoldenTicket);
			list.Add(item);
		}
		return new CompositeLocker(list);
	}
}
