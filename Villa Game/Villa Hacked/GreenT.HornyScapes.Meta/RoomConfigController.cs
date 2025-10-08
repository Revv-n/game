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
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		this.roomDataManager = roomDataManager;
		this.roomConfigManager = roomConfigManager;
		this.roomBundleLoader = roomBundleLoader;
		this.gameStarter = gameStarter;
	}

	public void Initialize()
	{
		gameActivityStream?.Dispose();
		gameActivityStream = ObservableExtensions.Subscribe<bool>((IObservable<bool>)gameStarter.IsGameActive, (Action<bool>)Track);
		RemoveLoadedRooms();
	}

	private void Track(bool gameIsActive)
	{
		trackOpenStream.Clear();
		if (!gameIsActive)
		{
			return;
		}
		IConnectableObservable<int> obj = Observable.Publish<int>(Observable.Merge<int>(roomDataManager.Collection.Where((RoomData _data) => !roomConfigManager.Collection.Any((RoomConfig _config) => _config.RoomID == _data.Id)).Select(EmitRoomIdOnUnlock)));
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<int>(Observable.Where<int>((IObservable<int>)obj, (Func<int, bool>)RoomIsLoaded), (Action<int>)delegate
		{
		}), (ICollection<IDisposable>)trackOpenStream);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<RoomConfig>(Observable.SelectMany<int, RoomConfig>(Observable.Where<int>((IObservable<int>)obj, (Func<int, bool>)RoomIsNotLoaded), (Func<int, IObservable<RoomConfig>>)roomBundleLoader.Load), (Action<RoomConfig>)roomConfigManager.Add, (Action<Exception>)delegate(Exception ex)
		{
			ex.LogException();
		}), (ICollection<IDisposable>)trackOpenStream);
		DisposableExtensions.AddTo<IDisposable>(obj.Connect(), (ICollection<IDisposable>)trackOpenStream);
		static IObservable<int> EmitRoomIdOnUnlock(RoomData _data)
		{
			return Observable.Select<bool, int>(Observable.First<bool>(Observable.Where<bool>((IObservable<bool>)_data.Lock.IsOpen, (Func<bool, bool>)((bool _isOpen) => _isOpen))), (Func<bool, int>)((bool _) => _data.Id));
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
		return Observable.AsSingleUnitObservable<RoomConfig>(Observable.Do<RoomConfig>(Observable.SelectMany<int, RoomConfig>(Observable.ToObservable<int>(first.Except(second)), (Func<int, IObservable<RoomConfig>>)roomBundleLoader.Load), (Action<RoomConfig>)roomConfigManager.Add));
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
