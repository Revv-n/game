using Merge.Meta.RoomObjects;
using UniRx;

namespace GreenT.HornyScapes.Meta.RoomObjects;

public interface IGameRoomObject<out T> : IRoomObject<T>, ITapable<IRoomObject<T>> where T : BaseObjectConfig
{
	ReactiveProperty<int> OnViewChanged { get; }

	RoomStateData Data { get; }

	void Highlight(HighlightType highlightType);
}
