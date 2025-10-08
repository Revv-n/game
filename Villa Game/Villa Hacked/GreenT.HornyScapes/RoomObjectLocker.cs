using GreenT.Types;
using StripClub.Model;

namespace GreenT.HornyScapes;

public class RoomObjectLocker : Locker
{
	public CompositeIdentificator RoomObjectID { get; }

	public int SkinId { get; }

	public RoomObjectLocker(int roomID, int objectID, int skinId)
	{
		RoomObjectID = new CompositeIdentificator(roomID, objectID);
		SkinId = skinId;
	}

	public void Set(int skinId)
	{
		isOpen.Value = skinId >= SkinId;
	}
}
