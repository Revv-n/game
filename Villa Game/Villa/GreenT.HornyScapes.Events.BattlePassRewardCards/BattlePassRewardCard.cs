using System;
using System.Collections.Generic;
using GreenT.HornyScapes._HornyScapes._Scripts.Events.ProgressWindow.BattlePassRewardCards;
using GreenT.HornyScapes.Characters.Skins.Content;
using Merge;
using Merge.Meta.RoomObjects;
using StripClub.Model;
using StripClub.UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Events.BattlePassRewardCards;

public class BattlePassRewardCard : MonoView<RewardWithManyConditions>
{
	private static readonly int DisabledOutline = EntityStatus.Rewarded.ConvertToInt();

	[SerializeField]
	private LocalizedTextMeshPro title;

	[SerializeField]
	private LocalizedTextMeshPro description;

	[SerializeField]
	private Image _backgroundImage;

	[SerializeField]
	private GameObject completeObject;

	[SerializeField]
	private Button claimButton;

	[SerializeField]
	private StatableComponentGroup _outlineStatable;

	[SerializeField]
	private Image resourceIcon;

	[SerializeField]
	private GameObject resourceIconObject;

	[SerializeField]
	private Image cardIcon;

	[SerializeField]
	private GameObject cardIconObject;

	[SerializeField]
	private StatableComponent[] rarityComponents;

	[SerializeField]
	private SpriteStates _lootboxStatable;

	[SerializeField]
	private PremiumConditionsLock premiumConditionsLock;

	[SerializeField]
	private DateConditionsLock dateConditionsLock;

	[SerializeField]
	private RewardAnimationView _rewardAnimationView;

	private Dictionary<Type, IConditionsLock> conditionsLocks;

	private IDisposable claimButtonSubscribe;

	private IDisposable stateStream;

	private void SetConditionsLocks()
	{
		if (conditionsLocks == null)
		{
			conditionsLocks = new Dictionary<Type, IConditionsLock>
			{
				{
					typeof(ConditionsReachingDate),
					dateConditionsLock
				},
				{
					typeof(ConditionsAchievingGoal),
					premiumConditionsLock
				}
			};
		}
	}

	private void Reset()
	{
		stateStream?.Dispose();
		claimButtonSubscribe?.Dispose();
		ResetAllConditionsLocks();
	}

	public override void Set(RewardWithManyConditions reward)
	{
		_outlineStatable.Set(DisabledOutline);
		SetConditionsLocks();
		Reset();
		base.Set(reward);
		if (reward == null)
		{
			Display(display: false);
			return;
		}
		if (IsCardType(reward.Content.Type))
		{
			CardTypeInitialization();
		}
		else
		{
			ResourceTypeInitialization();
		}
		TrySetLockers(reward);
		title.Init(reward.Content.GetName());
		description.Init(reward.Content.GetDescription());
		stateStream = base.Source.State.Subscribe(UpdateState);
		UpdateState(reward.State.Value);
		Display(display: true);
	}

	public override void Display(bool display)
	{
		if (!display)
		{
			Reset();
		}
		base.Display(display);
	}

	public void SetBackground(Sprite sprite)
	{
		_backgroundImage.sprite = sprite;
	}

	private void UpdateState(EntityStatus status)
	{
		if (base.Source.IsLight)
		{
			_outlineStatable.Set(status.ConvertToInt());
		}
		switch (status)
		{
		case EntityStatus.Blocked:
			SetBaseState();
			break;
		case EntityStatus.InProgress:
			SetInProgressState();
			break;
		case EntityStatus.Complete:
			SetCompleteState();
			break;
		case EntityStatus.Rewarded:
			SetRewardedState();
			break;
		default:
			throw new ArgumentOutOfRangeException("status", status, null);
		}
	}

	private void SetRewardedState()
	{
		completeObject.SetActive(value: true);
		claimButton.SetActive(active: false);
		ResetAllConditionsLocks();
		stateStream?.Dispose();
		claimButtonSubscribe?.Dispose();
	}

	private void SetBaseState()
	{
		claimButton.SetActive(active: false);
		completeObject.SetActive(value: false);
	}

	private void SetCompleteState()
	{
		claimButton.SetActive(active: true);
		completeObject.SetActive(value: false);
		claimButtonSubscribe = claimButton.OnClickAsObservable().Subscribe(delegate
		{
			base.Source.TryCollectReward();
		});
	}

	private void SetInProgressState()
	{
		completeObject.SetActive(value: false);
		claimButton.SetActive(active: false);
	}

	private void CardTypeInitialization()
	{
		cardIcon.sprite = base.Source.Icon;
		cardIconObject.SetActive(value: true);
		resourceIconObject.SetActive(value: false);
		if (_rewardAnimationView != null)
		{
			_rewardAnimationView.SetupAnimation(cardIconObject.transform);
		}
		StatableComponent[] array = rarityComponents;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(value: true);
		}
		if (rarityComponents != null && rarityComponents.Length != 0)
		{
			array = rarityComponents;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Set((int)base.Source.Rarity);
			}
		}
	}

	private void ResourceTypeInitialization()
	{
		if (base.Source.Content is LootboxLinkedContent lootboxLinkedContent)
		{
			_lootboxStatable.Set((int)lootboxLinkedContent.GetRarity());
		}
		else if (base.Source.Icon != null)
		{
			resourceIcon.sprite = base.Source.Icon;
		}
		cardIconObject.SetActive(value: false);
		resourceIconObject.SetActive(value: true);
		if (_rewardAnimationView != null)
		{
			_rewardAnimationView.SetupAnimation(resourceIcon.transform);
		}
		StatableComponent[] array = rarityComponents;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(value: false);
		}
	}

	private void TrySetLockers(RewardWithManyConditions reward)
	{
		EntityStatus value = base.Source.State.Value;
		if (value == EntityStatus.Complete || value == EntityStatus.Rewarded || conditionsLocks == null || conditionsLocks.Count == 0)
		{
			return;
		}
		IReadOnlyList<IConditionReceivingReward> conditions = reward.Conditions;
		if (conditions == null || conditions.Count == 0)
		{
			return;
		}
		foreach (IConditionReceivingReward item in conditions)
		{
			if (conditionsLocks.ContainsKey(item.Type))
			{
				conditionsLocks[item.Type].Initialization(item);
			}
		}
	}

	private bool IsCardType(Type type)
	{
		if (!(type == typeof(CardLinkedContent)))
		{
			return type == typeof(SkinLinkedContent);
		}
		return true;
	}

	private void ResetAllConditionsLocks()
	{
		if (conditionsLocks == null)
		{
			return;
		}
		foreach (IConditionsLock value in conditionsLocks.Values)
		{
			value.Reset();
		}
	}

	private void OnDestroy()
	{
		Reset();
	}
}
