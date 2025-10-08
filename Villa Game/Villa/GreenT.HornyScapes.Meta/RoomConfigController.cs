using System;
using System.Collections.Generic;
using System.Linq;
using Merge.Meta.RoomObjects;
using StripClub.Model.Data;
using UniRx;

namespace GreenT.HornyScapes.Meta;

public class RoomConfigController : IDisposable
{
	private CompositeDisposable trackOpenStream = new CompositeDisposable();

	private IDisposable gameActivityStream;

	private RoomDataManager roomDataManager;

	private RoomConfigManager roomConfigManager;

	private IBundlesLoader<int, RoomConfig> roomBundleLoader;

	private GameStarter gameStarter;

	public RoomConfigController(RoomDataManager roomDataManager, RoomConfigManager roomConfigManager, IBundlesLoader<int, RoomConfig> roomBundleLoader, GameStarter gameStarter)
	{
		this.roomDataManager = roomDataManager;
		this.roomConfigManager = roomConfigManager;
		this.roomBundleLoader = roomBundleLoader;
		this.gameStarter = gameStarter;
	}

	public void Initialize()
	{
		gameActivityStream?.Dispose();
		gameActivityStream = gameStarter.IsGameActive.Subscribe(Track);
		RemoveLoadedRooms();
	}

	private void Track(bool gameIsActive)
	{
		trackOpenStream.Clear();
		if (!gameIsActive)
		{
			return;
		}
		IConnectableObservable<int> connectableObservable = roomDataManager.Collection.Where((RoomData _data) => !roomConfigManager.Collection.Any((RoomConfig _config) => _config.RoomID == _data.Id)).Select(EmitRoomIdOnUnlock).Merge()
			.Publish();
		connectableObservable.Where(RoomIsLoaded).Subscribe(delegate
		{
		}).AddTo(trackOpenStream);
		connectableObservable.Where(RoomIsNotLoaded).SelectMany((Func<int, IObservable<RoomConfig>>)roomBundleLoader.Load).Subscribe(roomConfigManager.Add, delegate(Exception ex)
		{
			ex.LogException();
		})
			.AddTo(trackOpenStream);
		connectableObservable.Connect().AddTo(trackOpenStream);
		static IObservable<int> EmitRoomIdOnUnlock(RoomData _data)
		{
			return from _ in _data.Lock.IsOpen.Where((bool _isOpen) => _isOpen).First()
				select _data.Id;
		}
		bool RoomIsLoaded(int roomID)
		{
			return roomConfigManager.Collection.Any((RoomConfig _config) => _config.RoomID == roomID);
		}
		bool RoomIsNotLoaded(int roomID)
		{
			return !RoomIsLoaded(roomID);
		}
	}

	public IObservable<Unit> LoadUnlockedRooms()
	{
		RemoveLoadedRooms();
		IEnumerable<int> first = from _roomData in roomDataManager.Collection
			where _roomData.Preload || _roomData.Lock.IsOpen.Value
			select _roomData.Id;
		IEnumerable<int> second = roomConfigManager.Collection.Select((RoomConfig _room) => _room.RoomID);
		return first.Except(second).ToObservable().SelectMany((Func<int, IObservable<RoomConfig>>)roomBundleLoader.Load)
			.Do(roomConfigManager.Add)
			.AsSingleUnitObservable();
	}

	public void RemoveLoadedRooms()
	{
		int[] array = (from _data in roomDataManager.Collection
			where !_data.Preload && !_data.Lock.IsOpen.Value
			select _data.Id).ToArray();
		if (array.Any())
		{
			roomConfigManager.RemoveByIDs(array);
		}
	}

	public void Dispose()
	{
		trackOpenStream.Dispose();
		gameActivityStream?.Dispose();
	}
}
