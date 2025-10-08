using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Content;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.Sellouts.Models;
using GreenT.HornyScapes.Sellouts.Providers;
using GreenT.HornyScapes.Sellouts.Views;
using Merge.Meta.RoomObjects;
using StripClub.Model;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Sellouts.Services;

public class SelloutEntryPoint
{
	private RewardWithManyConditions[] _rewards;

	private IEnumerable<LinkedContent> _linkedContents;

	private readonly IContentAdder _contentAdder;

	private readonly SelloutManager _selloutManager;

	private readonly SelloutStateManager _selloutStateManager;

	[Inject]
	private SelloutEntryPoint(SelloutManager selloutManager, IContentAdder contentAdder, SelloutStateManager selloutStateManager)
	{
		_selloutManager = selloutManager;
		_contentAdder = contentAdder;
		_selloutStateManager = selloutStateManager;
	}

	public void Initialize()
	{
		_selloutStateManager.Deactivated.Subscribe(delegate
		{
			CheckSellouts();
		});
	}

	private void CheckSellouts()
	{
		PrepareRewards();
		GetRewards();
		ResetRewards();
	}

	private IEnumerable<LinkedContent> PrepareRewards()
	{
		_rewards = _selloutManager.Collection.SelectMany((Sellout sellout) => from reward in sellout.Rewards.SelectMany((SelloutRewardsInfo rewardInfo) => rewardInfo.PremiumRewards.Concat(rewardInfo.Rewards))
			where reward.State.Value == EntityStatus.Complete
			select reward).ToArray();
		_linkedContents = _rewards.Select((RewardWithManyConditions reward) => reward.Content);
		if (!_rewards.Any())
		{
			return LootboxContentExtensions.GetInnerContent(_linkedContents);
		}
		RewardWithManyConditions[] rewards = _rewards;
		for (int i = 0; i < rewards.Length; i++)
		{
			rewards[i].ForceSetState(EntityStatus.Rewarded);
		}
		return LootboxContentExtensions.GetInnerContent(_linkedContents);
	}

	private void GetRewards()
	{
		if (_linkedContents == null || !_linkedContents.Any())
		{
			return;
		}
		LinkedContent linkedContent = null;
		foreach (LinkedContent linkedContent2 in _linkedContents)
		{
			if (linkedContent == null)
			{
				linkedContent = linkedContent2;
			}
			else
			{
				linkedContent.Insert(linkedContent2);
			}
		}
		_contentAdder.AddContent(linkedContent);
	}

	private void ResetRewards()
	{
		RewardWithManyConditions[] rewards = _rewards;
		for (int i = 0; i < rewards.Length; i++)
		{
			rewards[i].ForceSetState(EntityStatus.InProgress);
		}
	}
}
