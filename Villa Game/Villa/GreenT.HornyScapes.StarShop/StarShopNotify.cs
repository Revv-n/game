using System.Linq;
using GreenT.HornyScapes.Tasks.UI;
using Merge.Meta.RoomObjects;
using UniRx;

namespace GreenT.HornyScapes.StarShop;

public class StarShopNotify : ControllerNotify<StarShopManager>
{
	private bool IsAnyTaskComplete => controller.Collection.Any((StarShopItem x) => x.State == EntityStatus.Complete);

	protected override void Awake()
	{
		base.Awake();
		SetState(IsAnyTaskComplete);
	}

	protected override void ListenEvents()
	{
		base.ListenEvents();
		controller.OnUpdate.Select((StarShopItem _) => IsAnyTaskComplete).Subscribe(base.SetState).AddTo(notifyStream);
	}
}
