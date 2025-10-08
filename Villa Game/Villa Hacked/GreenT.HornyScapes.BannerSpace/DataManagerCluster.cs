using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.Types;
using UniRx;

namespace GreenT.HornyScapes.BannerSpace;

public class DataManagerCluster : ContentCluster<CreateDataManager>
{
	private readonly Subject<CreateData> _onDataReady = new Subject<CreateData>();

	public IObservable<CreateData> OnDataReady => (IObservable<CreateData>)_onDataReady;

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
			ObservableExtensions.Subscribe<bool>(Observable.Take<bool>(Observable.Where<bool>((IObservable<bool>)creatData.Unlock.Locker.IsOpen, (Func<bool, bool>)((bool x) => x)), 1), (Action<bool>)delegate
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
