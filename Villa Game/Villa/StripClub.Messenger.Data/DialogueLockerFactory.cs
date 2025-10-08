using System;
using System.Collections.Generic;
using StripClub.Model;
using Zenject;

namespace StripClub.Messenger.Data;

public class DialogueLockerFactory : IFactory<DialogueConfigMapper, DialogueLocker>, IFactory
{
	private readonly IFactory<UnlockType, string, LockerSourceType, ILocker> lockerFactory;

	public DialogueLockerFactory(IFactory<UnlockType, string, LockerSourceType, ILocker> lockerFactory)
	{
		this.lockerFactory = lockerFactory;
	}

	public DialogueLocker Create(DialogueConfigMapper mapper)
	{
		IEnumerable<ILocker> lockers = CreateDialogueLockers(mapper);
		return new DialogueLocker(mapper.ID, lockers);
	}

	private IEnumerable<ILocker> CreateDialogueLockers(DialogueConfigMapper mapper)
	{
		if (mapper.UnlockTypes == null || mapper.UnlockValues == null || mapper.UnlockTypes.Length != mapper.UnlockValues.Length)
		{
			throw new ArgumentException("Error when creating dialogue: " + mapper.ID + ". Unlock types count (" + mapper.UnlockTypes.Length + ") must be equal unlock values count (" + mapper.UnlockValues.Length + ").");
		}
		ILocker[] array = new Locker[mapper.UnlockValues.Length];
		ILocker[] array2 = array;
		for (int i = 0; i != mapper.UnlockTypes.Length; i++)
		{
			array2[i] = lockerFactory.Create(mapper.UnlockTypes[i], mapper.UnlockValues[i], LockerSourceType.Dialogue);
		}
		return array2;
	}
}
