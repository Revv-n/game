using System;
using GreenT.Data;
using GreenT.Types;
using StripClub.Model;
using StripClub.Model.Data;
using Zenject;

namespace GreenT.HornyScapes.StarShop.Data;

public class StarShopFactory : IFactory<StarShopMapper, StarShopItem>, IFactory
{
	private readonly IFactory<UnlockType, string, LockerSourceType, ILocker> lockerFactory;

	private readonly ISaver saver;

	public StarShopFactory(IFactory<UnlockType, string, LockerSourceType, ILocker> lockerFactory, ISaver saver)
	{
		this.lockerFactory = lockerFactory;
		this.saver = saver;
	}

	public StarShopItem Create(StarShopMapper mapper)
	{
		ILocker[] lockers = CreateLockers(mapper);
		StarShopItem starShopItem = CreateItem(mapper, lockers);
		saver.Add(starShopItem);
		return starShopItem;
	}

	private ILocker[] CreateLockers(StarShopMapper mapper)
	{
		LockerExtension lockerExtension = new LockerExtension(lockerFactory);
		ILocker[] array = null;
		try
		{
			return lockerExtension.Create(mapper.unlock_type, mapper.unlock_value, LockerSourceType.StarShop);
		}
		catch (Exception innerException)
		{
			throw innerException.SendException($"{GetType().Name}: can't create locker for starshop with id: {mapper.step_id}");
		}
	}

	private StarShopItem CreateItem(StarShopMapper mapper, ILocker[] lockers)
	{
		try
		{
			Cost cost = new Cost(mapper.req_value, mapper.req_id);
			CompositeIdentificator houseObjectID = new CompositeIdentificator(mapper.room_id, mapper.object_number);
			return new StarShopItem(mapper.step_id, mapper.reward, houseObjectID, cost, lockers);
		}
		catch (Exception innerException)
		{
			throw innerException.SendException($"{GetType().Name}: Can't create StarShopItem with ID: {mapper.step_id}");
		}
	}
}
