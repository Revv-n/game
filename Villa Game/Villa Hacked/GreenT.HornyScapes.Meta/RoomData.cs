using StripClub.Model;

namespace GreenT.HornyScapes.Meta;

public class RoomData
{
	public bool Preload { get; }

	public int Id { get; }

	public ILocker Lock { get; }

	public RoomData(ILocker locker, int id, bool preload)
	{
		Lock = locker;
		Id = id;
		Preload = preload;
	}
}
