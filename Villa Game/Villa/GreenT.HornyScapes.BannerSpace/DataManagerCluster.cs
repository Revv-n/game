using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.Types;
using UniRx;

namespace GreenT.HornyScapes.BannerSpace;

public class DataManagerCluster : ContentCluster<CreateDataManager>
{
	private readonly Subject<CreateData> _onDataReady = new Subject<CreateData>();

	public IObservable<CreateData> OnDataReady => _onDataReady;

	public void Initialization()
	{
		HashSet<CreateData> hashSet = new HashSet<CreateData>();
		foreach (CreateData creatData in this.SelectMany((System.Collections.Generic.KeyValuePair<ContentType, CreateDataManager> pair) => pair.Value.Collection))
		{
			if (creatData.Unlock.Locker.IsOpen.Value)
			{
				hashSet.Add(creatData);
				continue;
			}
			creatData.Unlock.Locker.IsOpen.Where((bool x) => x).Take(1).Subscribe(delegate
			{
				_onDataReady.OnNext(creatData);
			});
		}
		foreach (CreateData item in hashSet)
		{
			_onDataReady.OnNext(item);
		}
	}
}
