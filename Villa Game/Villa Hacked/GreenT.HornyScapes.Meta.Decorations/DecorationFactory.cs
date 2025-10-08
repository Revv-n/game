using System;
using GreenT.Data;
using GreenT.HornyScapes.Lockers;
using GreenT.Types;
using StripClub.Model;
using Zenject;

namespace GreenT.HornyScapes.Meta.Decorations;

public class DecorationFactory : IFactory<DecorationMapper, Decoration>, IFactory
{
	private readonly IFactory<UnlockType, string, LockerSourceType, ILocker> _lockerFactory;

	private readonly ISaver _saver;

	public DecorationFactory(IFactory<UnlockType, string, LockerSourceType, ILocker> lockerFactory, ISaver saver)
	{
		_lockerFactory = lockerFactory;
		_saver = saver;
	}

	public Decoration Create(DecorationMapper mapper)
	{
		Decoration decoration = CreateRewardRoomObject(mapper);
		_saver.Add(decoration);
		return decoration;
	}

	private Decoration CreateRewardRoomObject(DecorationMapper mapper)
	{
		ILocker locker = CreateLocker(mapper.unlock_type, mapper.unlock_value);
		try
		{
			CompositeIdentificator houseObjectID = new CompositeIdentificator(mapper.room_id, mapper.object_number);
			return new Decoration(mapper.id, houseObjectID, locker);
		}
		catch (Exception innerException)
		{
			throw innerException.SendException($"{GetType().Name}: Can't create {typeof(Decoration)} with ID: {mapper.id}");
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
			array[i] = _lockerFactory.Create(unlock_type[i], unlock_value[i], LockerSourceType.Decoration);
		}
		return new CompositeLocker(array);
	}
}
