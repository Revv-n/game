using System;
using GreenT.HornyScapes.Meta;
using GreenT.HornyScapes.Meta.RoomObjects;
using GreenT.HornyScapes.UI;
using Merge.Meta.RoomObjects;
using UniRx;

namespace GreenT.HornyScapes.StarShop.SubSystems;

public class StarShopBoardFlow : IDisposable
{
	private const float BOARD_APPEARANCE_DELAY = 0.2f;

	private TimeSpan bordsDelayTime = TimeSpan.FromSeconds(0.20000000298023224);

	private CompositeDisposable itemStream = new CompositeDisposable();

	private StartFlow flow;

	private StarShopManager manager;

	private IHouseBuilder builder;

	private RoomManager roomManager;

	public StarShopBoardFlow(StartFlow flow, StarShopManager manager, IHouseBuilder builder, RoomManager roomManager)
	{
		this.flow = flow;
		this.manager = manager;
		this.builder = builder;
		this.roomManager = roomManager;
	}

	public void Activate(bool isOn)
	{
		ListenToStarItemChanges(isOn);
	}

	private void ListenToStarItemChanges(bool isOn)
	{
		itemStream.Clear();
		if (!isOn)
		{
			return;
		}
		IConnectableObservable<StarShopItem> connectableObservable = manager.OnUpdate.Publish();
		(from _item in connectableObservable.Where((StarShopItem _) => flow.IsLaunched).SelectMany((StarShopItem _item) => from _object in flow.OnSnapedTo
				where _item.HouseObjectID.Equals(_object.Config.ID)
				select _object into _
				select _item).Merge(connectableObservable.Where((StarShopItem _) => !flow.IsLaunched))
			where _item.State == EntityStatus.Rewarded
			select _item).Subscribe(SetRewardedState, delegate(Exception ex)
		{
			ex.LogException();
		}).AddTo(itemStream);
		(from _item in (from _item in connectableObservable.Where((StarShopItem _) => flow.IsLaunched).SelectMany((StarShopItem _item) => from _ in flow.OnUpdate.FirstOrDefault((IInputBlocker x) => !x.IsLaunched)
					select _item).Merge(connectableObservable.Where((StarShopItem _) => !flow.IsLaunched))
				where _item.State == EntityStatus.InProgress || _item.State == EntityStatus.Complete
				select _item).Delay(bordsDelayTime)
			where _item.State == EntityStatus.InProgress || _item.State == EntityStatus.Complete
			select _item).Subscribe(SetActiveState, delegate(Exception ex)
		{
			ex.LogException();
		}).AddTo(itemStream);
		connectableObservable.Connect().AddTo(itemStream);
	}

	private void SetActiveState(IStarShopItem item)
	{
		roomManager.GetObject(item.HouseObjectID).SetStatus(item.State);
	}

	public void SetRewardedState(StarShopItem item)
	{
		builder.BuildRoomObject(item.HouseObjectID);
	}

	public void Dispose()
	{
		itemStream.Dispose();
	}
}
