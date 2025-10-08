using System;
using System.Collections.Generic;
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
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<bool>(Observable.Select<StarShopItem, bool>(controller.OnUpdate, (Func<StarShopItem, bool>)((StarShopItem _) => IsAnyTaskComplete)), (Action<bool>)base.SetState), (ICollection<IDisposable>)notifyStream);
	}
}
