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

	public IObservable<SelloutRewardsInfo> ActiveRewardsChanged => _activeRewardsChanged.AsObservable();

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
		source.Rewards.ToObservable().SelectMany((SelloutRewardsInfo rewardInfo) => rewardInfo.Rewards.Concat(rewardInfo.PremiumRewards).FirstOrDefault().State).Merge()
			.Subscribe(delegate
			{
				EmitFirstInProgressReward();
			})
			.AddTo(_subscriptions);
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
			IObservable<EntityStatus>[] sources = (from reward in selloutRewardsInfo.Rewards.Concat(selloutRewardsInfo.PremiumRewards)
				select reward.State).ToArray();
			(from hasComplete in (from states in Observable.CombineLatest(sources)
					select states.Any((EntityStatus state) => state == EntityStatus.Complete)).DistinctUntilChanged().Skip(1)
				where hasComplete
				select hasComplete).Subscribe(delegate
			{
				_selloutAnalytic.SendRewardReceivedEvent(sellout.ID, rewardIndex);
			}).AddTo(_subscriptions);
		}
	}
}
