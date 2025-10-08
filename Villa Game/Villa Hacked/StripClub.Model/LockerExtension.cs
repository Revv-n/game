using System;
using GreenT;
using Zenject;

namespace StripClub.Model;

public class LockerExtension
{
	private readonly IFactory<UnlockType, string, LockerSourceType, ILocker> lockerFactory;

	public LockerExtension(IFactory<UnlockType, string, LockerSourceType, ILocker> lockerFactory)
	{
		this.lockerFactory = lockerFactory;
	}

	public ILocker[] Create(UnlockType[] unlock_type, string[] unlock_value, LockerSourceType sourceType)
	{
		int i = 0;
		ILocker[] array = new ILocker[unlock_type.Length];
		try
		{
			for (; i < array.Length; i++)
			{
				array[i] = lockerFactory.Create(unlock_type[i], unlock_value[i], sourceType);
			}
			return array;
		}
		catch (Exception innerException)
		{
			throw innerException.SendException(GetType().Name + ": Can't create locker: " + unlock_type[i].ToString() + " with value: " + unlock_value[i]);
		}
	}
}
