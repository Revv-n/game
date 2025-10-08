using System;
using GreenT.HornyScapes.Meta.RoomObjects;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Meta.Navigation;

public class HouseNavigationController : NavigationController
{
	[Range(0.5f, 2f)]
	[SerializeField]
	private float snapMultiplicator = 1f;

	private HouseBackgroundBuilder backgroundBuilder;

	private RoomManager house;

	[Inject]
	public void Init(HouseBackgroundBuilder backgroundBuilder, RoomManager house)
	{
		this.backgroundBuilder = backgroundBuilder;
		this.house = house;
	}

	public override void Start()
	{
		base.Start();
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Bounds>(backgroundBuilder.HouseBounds, (Action<Bounds>)base.SetMovementBounds), (Component)this);
	}

	public void SnapToRoom(Room room)
	{
		Bounds bounds = room.GetBounds();
		bounds.Expand(bounds.size * (snapMultiplicator - 1f));
		SnapTo(bounds);
	}

	public void SnapToObject(IRoomObject<BaseObjectConfig> roomObject)
	{
		if (roomObject.Config.Number == 0)
		{
			Room roomOrDefault = house.GetRoomOrDefault(roomObject.Config.RoomID);
			SnapToRoom(roomOrDefault);
		}
		else
		{
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Bounds>(roomObject.GetBounds(), (Action<Bounds>)SnapTo), (Component)this);
		}
	}
}
