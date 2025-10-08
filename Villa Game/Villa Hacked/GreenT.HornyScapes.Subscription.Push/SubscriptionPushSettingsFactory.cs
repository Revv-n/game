using System;
using System.Collections.Generic;
using System.Linq;
using StripClub.Model;
using StripClub.Model.Shop;
using Zenject;

namespace GreenT.HornyScapes.Subscription.Push;

public class SubscriptionPushSettingsFactory : IFactory<SubscriptionPushMapper, SubscriptionPushSettings>, IFactory
{
	private readonly LotManager _lotManager;

	private readonly LockerFactory _lockerFactory;

	public SubscriptionPushSettingsFactory(LockerFactory lockerFactory, LotManager lotManager)
	{
		_lockerFactory = lockerFactory;
		_lotManager = lotManager;
	}

	public SubscriptionPushSettings Create(SubscriptionPushMapper mapper)
	{
		try
		{
			SubscriptionLot[] array = GetSubscriptions(mapper.subscription_id).ToArray();
			CompositeLocker locker = CreateCompositeLocker(mapper, array);
			return new SubscriptionPushSettings(mapper, array, locker);
		}
		catch (Exception innerException)
		{
			throw innerException.SendException("Ошибка при парсинге настроек пуша подписки с id: " + mapper.id);
		}
	}

	protected CompositeLocker CreateCompositeLocker(SubscriptionPushMapper mapper, SubscriptionLot[] lot)
	{
		List<ILocker> list = new List<ILocker>();
		for (int i = 0; i != mapper.unlock_type.Length; i++)
		{
			ILocker item = _lockerFactory.Create(mapper.unlock_type[i], mapper.unlock_value[i], LockerSourceType.OfferTab);
			list.Add(item);
		}
		for (int j = 0; j != lot.Length; j++)
		{
			ILocker locker = lot[j].Locker;
			list.Add(locker);
		}
		return new CompositeLocker(list);
	}

	private List<SubscriptionLot> GetSubscriptions(int[] subscriptionIDs)
	{
		SubscriptionLot[] source = _lotManager.GetLot<SubscriptionLot>().ToArray();
		List<SubscriptionLot> list = new List<SubscriptionLot>();
		for (int i = 0; i < subscriptionIDs.Length; i++)
		{
			int id = subscriptionIDs[i];
			try
			{
				SubscriptionLot item = source.First((SubscriptionLot sub) => sub.ID == id);
				list.Add(item);
			}
			catch (Exception innerException)
			{
				throw innerException.SendException("В коллекции лотов нет подписки с ID: " + id);
			}
		}
		return list;
	}
}
