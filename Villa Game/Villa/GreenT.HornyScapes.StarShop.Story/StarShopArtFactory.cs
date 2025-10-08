using System;
using StripClub.Model;
using Zenject;

namespace GreenT.HornyScapes.StarShop.Story;

public class StarShopArtFactory : IFactory<StarShopArtMapper, StarShopArt>, IFactory
{
	private readonly IFactory<UnlockType, string, LockerSourceType, ILocker> lockerFactory;

	public StarShopArtFactory(IFactory<UnlockType, string, LockerSourceType, ILocker> lockerFactory)
	{
		this.lockerFactory = lockerFactory;
	}

	public StarShopArt Create(StarShopArtMapper mapper)
	{
		ILocker[] array = new ILocker[mapper.unlock_type.Length];
		try
		{
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = lockerFactory.Create(mapper.unlock_type[i], mapper.unlock_value[i], LockerSourceType.StarStory);
			}
		}
		catch (Exception innerException)
		{
			throw innerException.SendException(GetType().Name + ": Can't create locker for StarStoryID " + mapper.id);
		}
		try
		{
			return new StarShopArt(mapper.id, array);
		}
		catch (Exception innerException2)
		{
			throw innerException2.SendException($"{GetType().Name}: Can't create StarStory with ID: {mapper.id}");
		}
	}
}
