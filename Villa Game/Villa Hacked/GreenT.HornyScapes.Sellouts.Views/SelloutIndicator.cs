using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.Sellouts.Models;
using GreenT.HornyScapes.Sellouts.Providers;
using Merge.Meta.RoomObjects;
using StripClub.UI;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Sellouts.Views;

public sealed class SelloutIndicator : MonoView
{
	private SelloutManager _selloutManager;

	private readonly CompositeDisposable _disposables = new CompositeDisposable();

	[Inject]
	private void Init(SelloutManager selloutManager)
	{
		_selloutManager = selloutManager;
	}

	private void Start()
	{
		InitializeIndicator();
	}

	private void InitializeIndicator()
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Sellout>(_selloutManager.OnNew, (Action<Sellout>)delegate
		{
			UpdateIndicator();
		}), (ICollection<IDisposable>)_disposables);
		UpdateIndicator();
	}

	private void UpdateIndicator()
	{
		if (_selloutManager.Collection == null || !_selloutManager.Collection.Any())
		{
			base.gameObject.SetActive(value: false);
			return;
		}
		IReadOnlyReactiveProperty<bool>[] array = _selloutManager.Collection.Select((Sellout sellout) => sellout.IsFirstShowed).ToArray();
		IObservable<bool> observable;
		if (array.Length == 0)
		{
			observable = Observable.Return(false);
		}
		else
		{
			IObservable<bool>[] array2 = (IObservable<bool>[])array;
			observable = Observable.Select<IList<bool>, bool>(Observable.CombineLatest<bool>(array2), (Func<IList<bool>, bool>)((IList<bool> list) => list.Any((bool x) => !x)));
		}
		IObservable<bool> observable2 = observable;
		IObservable<bool>[] array3 = _selloutManager.Collection.Select(delegate(Sellout sellout)
		{
			IObservable<EntityStatus>[] array4 = (IObservable<EntityStatus>[])(from reward in sellout.Rewards.SelectMany((SelloutRewardsInfo rewardInfo) => rewardInfo.PremiumRewards.Concat(rewardInfo.Rewards))
				select reward.State).ToArray();
			return Observable.Select<IList<EntityStatus>, bool>(Observable.CombineLatest<EntityStatus>(array4), (Func<IList<EntityStatus>, bool>)((IList<EntityStatus> states) => states.Any((EntityStatus state) => state == EntityStatus.Complete)));
		}).ToArray();
		IObservable<bool> observable3 = ((array3.Length != 0) ? Observable.Select<IList<bool>, bool>(Observable.CombineLatest<bool>(array3), (Func<IList<bool>, bool>)((IList<bool> list) => list.Any((bool x) => x))) : Observable.Return(false));
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<IList<bool>>(Observable.DistinctUntilChanged<IList<bool>>(Observable.CombineLatest<bool>(new IObservable<bool>[2] { observable2, observable3 })), (Action<IList<bool>>)delegate(IList<bool> results)
		{
			bool num = results[0];
			bool flag = results[1];
			bool active = num || flag;
			base.gameObject.SetActive(active);
		}), (ICollection<IDisposable>)_disposables);
	}

	private void OnDestroy()
	{
		_disposables.Dispose();
	}
}
