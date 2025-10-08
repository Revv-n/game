using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Meta.Data;
using StripClub.Model.Data;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Meta;

public class RoomDataLoader : ILoader<IEnumerable<RoomData>>
{
	private ILoader<HouseConfig> houseLoader;

	private IFactory<RoomDataConfig, RoomData> roomDataFactory;

	public RoomDataLoader(ILoader<HouseConfig> houseLoader, IFactory<RoomDataConfig, RoomData> roomDataFactory)
	{
		this.houseLoader = houseLoader;
		this.roomDataFactory = roomDataFactory;
	}

	public IObservable<IEnumerable<RoomData>> Load()
	{
		return (from _house in houseLoader.Load().Debug("House: Load main house config", LogType.BundleLoad)
			select _house.RoomDatas).SelectMany((RoomDataConfig[] x) => x).Select(roomDataFactory.Create).ToArray()
			.Catch(delegate(Exception ex)
			{
				throw ex.SendException(GetType().Name + ": Error when creating \n");
			});
	}
}
