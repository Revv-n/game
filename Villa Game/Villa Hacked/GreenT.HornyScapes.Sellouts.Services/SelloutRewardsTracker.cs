using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.Sellouts.Analytics;
using GreenT.HornyScapes.Sellouts.Models;
using Merge.Meta.RoomObjects;
using StripClub.UI;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Sellouts.Services;

public class SelloutRewardsTracker : MonoView<Sellout>, IDisposable
{
	private SelloutAnalytic _selloutAnalytic;

	private readonly CompositeDisposable _subscriptions = new CompositeDisposable();

	private readonly Subject<SelloutRewardsInfo> _activeRewardsChanged = new Subject<SelloutRewardsInfo>();

	public IObservable<SelloutRewardsInfo> ActiveRewardsChanged => Observable.AsObservable<SelloutRewardsInfo>((IObservable<SelloutRewardsInfo>)_activeRewardsChanged);

	[Inject]
	private void Init(SelloutAnalytic selloutAnalytic)
	{
		_selloutAnalytic = selloutAnalytic;
	}

	public override void Set(Sellout source)
	{
		base.Set(source);
		_subscriptions.Clear();
		EmitFirstInProgressReward();
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<EntityStatus>(Observable.Merge<EntityStatus>(Observable.SelectMany<SelloutRewardsInfo, EntityStatus>(Observable.ToObservable<SelloutRewardsInfo>((IEnumerable<SelloutRewardsInfo>)source.Rewards), (Func<SelloutRewardsInfo, IObservable<EntityStatus>>)((SelloutRewardsInfo rewardInfo) => (IObservable<EntityStatus>)rewardInfo.Rewards.Concat(rewardInfo.PremiumRewards).FirstOrDefault().State)), Array.Empty<IObservable<EntityStatus>>()), (Action<EntityStatus>)delegate
		{
			EmitFirstInProgressReward();
		}), (ICollection<IDisposable>)_subscriptions);
		SubscribeToRewardStateChanges(source);
	}

	public void Dispose()
	{
		_subscriptions.Dispose();
		_activeRewardsChanged.Dispose();
	}

	private void OnDestroy()
	{
		Dispose();
	}

	private void EmitFirstInProgressReward()
	{
		SelloutRewardsInfo selloutRewardsInfo = base.Source.Rewards.FirstOrDefault((SelloutRewardsInfo rewardInfo) => rewardInfo.Rewards.Concat(rewardInfo.PremiumRewards).All((RewardWithManyConditions reward) => reward.State.Value == EntityStatus.InProgress));
		if (selloutRewardsInfo != null)
		{
			_activeRewardsChanged.OnNext(selloutRewardsInfo);
		}
	}

	private void SubscribeToRewardStateChanges(Sellout sellout)
	{
		for (int i = 0; i < sellout.Rewards.Count; i++)
		{
			int rewardIndex = i + 1;
			SelloutRewardsInfo selloutRewardsInfo = sellout.Rewards[i];
			IObservable<EntityStatus>[] array = (IObservable<EntityStatus>[])(from reward in selloutRewardsInfo.Rewards.Concat(selloutRewardsInfo.PremiumRewards)
				select reward.State).ToArray();
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<bool>(Observable.Where<bool>(Observable.Skip<bool>(Observable.DistinctUntilChanged<bool>(Observable.Select<IList<EntityStatus>, bool>(Observable.CombineLatest<EntityStatus>(array), (Func<IList<EntityStatus>, bool>)((IList<EntityStatus> states) => states.Any((EntityStatus state) => state == EntityStatus.Complete)))), 1), (Func<bool, bool>)((bool hasComplete) => hasComplete)), (Action<bool>)delegate
			{
				_selloutAnalytic.SendRewardReceivedEvent(sellout.ID, rewardIndex);
			}), (ICollection<IDisposable>)_subscriptions);
		}
	}
}
