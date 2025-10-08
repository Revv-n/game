using Merge.Meta.RoomObjects;

namespace GreenT.HornyScapes.Meta.RoomObjects;

public interface IRoomObjectInitializer<in T> where T : BaseObjectConfig
{
	void Init(RoomStateData data, T config);
}
