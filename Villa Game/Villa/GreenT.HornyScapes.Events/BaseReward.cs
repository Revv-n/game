using System;
using Merge.Meta.RoomObjects;
using StripClub.Model;
using UniRx;

namespace GreenT.HornyScapes.Events;

public class BaseReward : IDisposable
{
	public readonly LinkedContent Content;

	public readonly int Target;

	public readonly ReactiveProperty<EntityStatus> State;

	public readonly BaseReward PrevReward;

	public BaseReward(LinkedContent content, int target, BaseReward prevReward)
	{
		Content = content;
		Target = target;
		State = new ReactiveProperty<EntityStatus>(EntityStatus.Blocked);
		PrevReward = prevReward;
	}

	public virtual void SetInProgress()
	{
		if (Content is LootboxLinkedContent lootboxLinkedContent)
		{
			lootboxLinkedContent.Lootbox.PrepareContent();
		}
		State.Value = EntityStatus.InProgress;
	}

	public virtual void SetComplete()
	{
		if (Content is LootboxLinkedContent lootboxLinkedContent && !lootboxLinkedContent.Lootbox.HasPreopenedContent)
		{
			lootboxLinkedContent.Lootbox.PrepareContent();
		}
		State.Value = EntityStatus.Complete;
	}

	public virtual void SetRewarded()
	{
		State.Value = EntityStatus.Rewarded;
	}

	public virtual void SetBlocked()
	{
		State.Value = EntityStatus.Blocked;
	}

	public virtual void Dispose()
	{
		State?.Dispose();
	}
}
