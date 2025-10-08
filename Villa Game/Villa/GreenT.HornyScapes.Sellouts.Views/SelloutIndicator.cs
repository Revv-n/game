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
		_selloutManager.OnNew.Subscribe(delegate
		{
			UpdateIndicator();
		}).AddTo(_disposables);
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
			observable = Observable.Return(value: false);
		}
		else
		{
			IObservable<bool>[] sources = array;
			observable = from list in Observable.CombineLatest(sources)
				select list.Any((bool x) => !x);
		}
		IObservable<bool> observable2 = observable;
		IObservable<bool>[] array2 = _selloutManager.Collection.Select(delegate(Sellout sellout)
		{
			IObservable<EntityStatus>[] sources2 = (from reward in sellout.Rewards.SelectMany((SelloutRewardsInfo rewardInfo) => rewardInfo.PremiumRewards.Concat(rewardInfo.Rewards))
				select reward.State).ToArray();
			return from states in Observable.CombineLatest(sources2)
				select states.Any((EntityStatus state) => state == EntityStatus.Complete);
		}).ToArray();
		IObservable<bool> observable3 = ((array2.Length != 0) ? (from list in Observable.CombineLatest(array2)
			select list.Any((bool x) => x)) : Observable.Return(value: false));
		Observable.CombineLatest<bool>(observable2, observable3).DistinctUntilChanged().Subscribe(delegate(IList<bool> results)
		{
			bool num = results[0];
			bool flag = results[1];
			bool active = num || flag;
			base.gameObject.SetActive(active);
		})
			.AddTo(_disposables);
	}

	private void OnDestroy()
	{
		_disposables.Dispose();
	}
}
