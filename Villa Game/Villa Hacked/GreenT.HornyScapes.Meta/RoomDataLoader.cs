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
		return Observable.Catch<RoomData[], Exception>(Observable.ToArray<RoomData>(Observable.Select<RoomDataConfig, RoomData>(Observable.SelectMany<RoomDataConfig[], RoomDataConfig>(Observable.Select<HouseConfig, RoomDataConfig[]>(houseLoader.Load().Debug("House: Load main house config", LogType.BundleLoad), (Func<HouseConfig, RoomDataConfig[]>)((HouseConfig _house) => _house.RoomDatas)), (Func<RoomDataConfig[], IEnumerable<RoomDataConfig>>)((RoomDataConfig[] x) => x)), (Func<RoomDataConfig, RoomData>)roomDataFactory.Create)), (Func<Exception, IObservable<RoomData[]>>)delegate(Exception ex)
		{
			throw ex.SendException(GetType().Name + ": Error when creating \n");
		});
	}
}
