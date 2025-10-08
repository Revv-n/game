using System;
using System.Collections.Generic;
using System.Linq;
using Merge.Meta.RoomObjects;
using StripClub.Model.Data;
using UniRx;

namespace GreenT.HornyScapes.Meta;

public class RoomConfigLoader : IBundlesLoader<IEnumerable<int>, IEnumerable<RoomConfig>>, ILoader<IEnumerable<int>, IEnumerable<RoomConfig>>
{
	private IBundlesLoader<int, RoomConfig> roomBundleLoader;

	public RoomConfigLoader(IBundlesLoader<int, RoomConfig> roomBundleLoader)
	{
		this.roomBundleLoader = roomBundleLoader;
	}

	public IObservable<IEnumerable<RoomConfig>> Load(IEnumerable<int> ids)
	{
		return ids.ToObservable().SelectMany((Func<int, IObservable<RoomConfig>>)roomBundleLoader.Load).Buffer(ids.Count());
	}

	public void ReleaseBundle(IEnumerable<int> ids)
	{
		foreach (int id in ids)
		{
			roomBundleLoader.ReleaseBundle(id);
		}
	}
}
