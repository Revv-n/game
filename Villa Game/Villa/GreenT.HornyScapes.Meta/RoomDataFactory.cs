using GreenT.HornyScapes.Meta.Data;
using StripClub.Model;
using Zenject;

namespace GreenT.HornyScapes.Meta;

public class RoomDataFactory : IFactory<RoomDataConfig, RoomData>, IFactory
{
	private readonly IFactory<UnlockType, string, LockerSourceType, ILocker> lockerFactory;

	public RoomDataFactory(IFactory<UnlockType, string, LockerSourceType, ILocker> lockerFactory)
	{
		this.lockerFactory = lockerFactory;
	}

	public RoomData Create(RoomDataConfig roomDataConfig)
	{
		return new RoomData(lockerFactory.Create(roomDataConfig.UnlockType, roomDataConfig.UnlockValue, LockerSourceType.RoomData), roomDataConfig.RoomId, roomDataConfig.IsPreload);
	}
}
