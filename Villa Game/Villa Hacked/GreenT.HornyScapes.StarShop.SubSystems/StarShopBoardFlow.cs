using System;
using System.Collections.Generic;
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
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
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
		IConnectableObservable<StarShopItem> val = Observable.Publish<StarShopItem>(manager.OnUpdate);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<StarShopItem>(Observable.Where<StarShopItem>(Observable.Merge<StarShopItem>(Observable.SelectMany<StarShopItem, StarShopItem>(Observable.Where<StarShopItem>((IObservable<StarShopItem>)val, (Func<StarShopItem, bool>)((StarShopItem _) => flow.IsLaunched)), (Func<StarShopItem, IObservable<StarShopItem>>)((StarShopItem _item) => Observable.Select<IRoomObject<BaseObjectConfig>, StarShopItem>(Observable.Where<IRoomObject<BaseObjectConfig>>(flow.OnSnapedTo, (Func<IRoomObject<BaseObjectConfig>, bool>)((IRoomObject<BaseObjectConfig> _object) => _item.HouseObjectID.Equals(_object.Config.ID))), (Func<IRoomObject<BaseObjectConfig>, StarShopItem>)((IRoomObject<BaseObjectConfig> _) => _item)))), new IObservable<StarShopItem>[1] { Observable.Where<StarShopItem>((IObservable<StarShopItem>)val, (Func<StarShopItem, bool>)((StarShopItem _) => !flow.IsLaunched)) }), (Func<StarShopItem, bool>)((StarShopItem _item) => _item.State == EntityStatus.Rewarded)), (Action<StarShopItem>)SetRewardedState, (Action<Exception>)delegate(Exception ex)
		{
			ex.LogException();
		}), (ICollection<IDisposable>)itemStream);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<StarShopItem>(Observable.Where<StarShopItem>(Observable.Delay<StarShopItem>(Observable.Where<StarShopItem>(Observable.Merge<StarShopItem>(Observable.SelectMany<StarShopItem, StarShopItem>(Observable.Where<StarShopItem>((IObservable<StarShopItem>)val, (Func<StarShopItem, bool>)((StarShopItem _) => flow.IsLaunched)), (Func<StarShopItem, IObservable<StarShopItem>>)((StarShopItem _item) => Observable.Select<IInputBlocker, StarShopItem>(Observable.FirstOrDefault<IInputBlocker>(flow.OnUpdate, (Func<IInputBlocker, bool>)((IInputBlocker x) => !x.IsLaunched)), (Func<IInputBlocker, StarShopItem>)((IInputBlocker _) => _item)))), new IObservable<StarShopItem>[1] { Observable.Where<StarShopItem>((IObservable<StarShopItem>)val, (Func<StarShopItem, bool>)((StarShopItem _) => !flow.IsLaunched)) }), (Func<StarShopItem, bool>)((StarShopItem _item) => _item.State == EntityStatus.InProgress || _item.State == EntityStatus.Complete)), bordsDelayTime), (Func<StarShopItem, bool>)((StarShopItem _item) => _item.State == EntityStatus.InProgress || _item.State == EntityStatus.Complete)), (Action<StarShopItem>)SetActiveState, (Action<Exception>)delegate(Exception ex)
		{
			ex.LogException();
		}), (ICollection<IDisposable>)itemStream);
		DisposableExtensions.AddTo<IDisposable>(val.Connect(), (ICollection<IDisposable>)itemStream);
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
