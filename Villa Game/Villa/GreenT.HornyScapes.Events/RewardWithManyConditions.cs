using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Content;
using GreenT.HornyScapes.Lootboxes;
using Merge.Meta.RoomObjects;
using StripClub.Model;
using StripClub.Model.Cards;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Events;

public class RewardWithManyConditions : IDisposable
{
	public readonly Selector Selector;

	public readonly LinkedContent Content;

	public readonly ReactiveProperty<EntityStatus> State;

	public readonly string SaveKey;

	public readonly bool IsLight;

	private readonly IConditionReceivingReward[] conditions;

	private readonly Sprite bundleTarget;

	private readonly IContentAdder contentAdder;

	private readonly IDisposable stateStream;

	private CompositeDisposable conditionDisposables;

	private Sprite icon;

	private readonly Subject<RewardWithManyConditions> onUpdate = new Subject<RewardWithManyConditions>();

	public IObservable<RewardWithManyConditions> OnUpdate => onUpdate.AsObservable();

	public Sprite Icon => GetIcon();

	public Rarity Rarity => GetRarity();

	public IReadOnlyList<IConditionReceivingReward> Conditions => conditions;

	public bool IsRewarded => State.Value == EntityStatus.Rewarded;

	public bool IsInProgress => State.Value == EntityStatus.InProgress;

	public bool IsComplete => State.Value == EntityStatus.Complete;

	public bool IsBlocked => State.Value == EntityStatus.Blocked;

	public RewardWithManyConditions(string saveKey, LinkedContent content, IConditionReceivingReward[] conditions, IContentAdder contentAdder, Selector selector, bool isLight = false)
	{
		this.conditions = conditions;
		this.contentAdder = contentAdder;
		IsLight = isLight;
		Content = content;
		SaveKey = saveKey;
		Selector = selector;
		State = new ReactiveProperty<EntityStatus>(EntityStatus.Blocked);
		stateStream = State.Subscribe(delegate
		{
			onUpdate.OnNext(this);
		});
		TrySetInProgress();
	}

	private Sprite GetIcon()
	{
		if (icon != null)
		{
			return icon;
		}
		if (!Content.TryGetAlternativeIcon(out icon))
		{
			icon = Content.GetIcon();
		}
		return icon;
	}

	private Rarity GetRarity()
	{
		return Content.GetRarity();
	}

	public bool TrySetInProgress()
	{
		if (!IsBlocked)
		{
			return false;
		}
		ActivateConditions();
		State.Value = EntityStatus.InProgress;
		if (Validate())
		{
			TrySetComplete();
			return true;
		}
		conditionDisposables?.Dispose();
		conditionDisposables = new CompositeDisposable();
		foreach (IConditionReceivingReward item in conditions.Where((IConditionReceivingReward p) => !p.Validate()))
		{
			item.State.Where((ConditionState p) => p == ConditionState.Completed).Subscribe(delegate
			{
				TrySetComplete();
			}).AddTo(conditionDisposables);
		}
		return true;
	}

	public bool TryCollectReward(bool isFast = false)
	{
		if (!IsComplete)
		{
			return false;
		}
		if (!isFast)
		{
			contentAdder.AddContent(Content);
		}
		SetRewarded();
		return true;
	}

	public void ResetState()
	{
		State.Value = EntityStatus.InProgress;
		ActivateConditions();
		if (Validate())
		{
			TrySetComplete();
		}
		conditionDisposables = new CompositeDisposable();
		foreach (IConditionReceivingReward item in conditions.Where((IConditionReceivingReward p) => !p.Validate()))
		{
			item.State.Where((ConditionState p) => p == ConditionState.Completed).Subscribe(delegate
			{
				TrySetComplete();
			}).AddTo(conditionDisposables);
		}
	}

	private void TrySetComplete()
	{
		if (IsInProgress && Validate())
		{
			conditionDisposables?.Dispose();
			ResetConditions();
			State.Value = EntityStatus.Complete;
		}
	}

	public void ForceSetState(EntityStatus status)
	{
		switch (status)
		{
		case EntityStatus.Blocked:
			SetBlocked();
			break;
		case EntityStatus.InProgress:
			SetInProgress();
			break;
		case EntityStatus.Complete:
			SetComplete();
			break;
		case EntityStatus.Rewarded:
			SetRewarded();
			break;
		default:
			throw new ArgumentOutOfRangeException("status", status, null);
		}
	}

	private void SetInProgress()
	{
		State.Value = EntityStatus.InProgress;
	}

	private void SetComplete()
	{
		State.Value = EntityStatus.Complete;
	}

	private void SetRewarded()
	{
		State.Value = EntityStatus.Rewarded;
	}

	private void SetBlocked()
	{
		State.Value = EntityStatus.Blocked;
	}

	private bool Validate()
	{
		if (!conditions.All((IConditionReceivingReward p) => p.Validate()) && !IsRewarded)
		{
			return IsComplete;
		}
		return true;
	}

	private void ActivateConditions()
	{
		IConditionReceivingReward[] array = conditions;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Activate();
		}
	}

	private void ResetConditions()
	{
		IConditionReceivingReward[] array = conditions;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Reset();
		}
	}

	public virtual void Dispose()
	{
		State?.Dispose();
		stateStream?.Dispose();
		conditionDisposables?.Dispose();
	}
}
