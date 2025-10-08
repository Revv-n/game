using System;
using System.Linq;
using GreenT.HornyScapes.StarShop;
using Merge.Meta.RoomObjects;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Tasks.UI;

public class TaskBookNotify : BaseNotify
{
	private StarShopManager starShopManager;

	private Subject<bool> onUpdate = new Subject<bool>();

	public IObservable<bool> OnUpdate => onUpdate.AsObservable();

	private bool IsAnyTaskComplete => starShopManager.Collection.Any((StarShopItem x) => x.State == EntityStatus.Complete);

	[Inject]
	protected void InnerInit(StarShopManager manager)
	{
		starShopManager = manager;
	}

	private void Awake()
	{
		ActivateNotify();
		ListenEvents();
	}

	public void ActivateNotify()
	{
		bool isAnyTaskComplete = IsAnyTaskComplete;
		SetState(isAnyTaskComplete);
		onUpdate.OnNext(isAnyTaskComplete);
	}

	protected override void ListenEvents()
	{
		base.ListenEvents();
		starShopManager.OnUpdate.Subscribe(delegate
		{
			ActivateNotify();
		}).AddTo(notifyStream);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		onUpdate.Dispose();
	}
}
