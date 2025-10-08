using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.MergeCore;
using Merge;
using UnityEngine;

namespace GreenT.HornyScapes.Tasks;

[Objective]
public class OneWayMergeItemObjective : SavableObjective
{
	private readonly MergeIconService _iconProvider;

	public GIKey ItemKey { get; }

	public override bool IsComplete => Data.Progress >= Data.Required;

	public OneWayMergeItemObjective(GIKey key, SavableObjectiveData data, MergeIconService iconProvider)
		: base(data)
	{
		_iconProvider = iconProvider;
		ItemKey = key;
	}

	public override Sprite GetIcon()
	{
		return _iconProvider.GetSprite(ItemKey);
	}

	public override int GetProgress()
	{
		return Data.Progress;
	}

	public override int GetTarget()
	{
		return Data.Required;
	}

	public override void Track()
	{
		GameItemController instance = Controller<GameItemController>.Instance;
		Controller<BubbleController>.Instance.OnBubbleUnlock += OnItemCreated;
		Controller<LockedController>.Instance.OnItemActionUnlock += OnItemCreated;
		instance.OnItemCreated += OnItemCreated;
		onUpdate.OnNext((IObjective)this);
	}

	public override void OnRewardTask()
	{
		GameItemController instance = Controller<GameItemController>.Instance;
		Controller<BubbleController>.Instance.OnBubbleUnlock -= OnItemCreated;
		Controller<LockedController>.Instance.OnItemActionUnlock -= OnItemCreated;
		instance.OnItemCreated -= OnItemCreated;
	}

	protected virtual void OnItemCreated(GameItem item)
	{
		if (!(item.Key != ItemKey) && Controller<GameItemController>.Instance.IsAvailableItem(item))
		{
			IncreaseProgress(item);
		}
	}

	protected void IncreaseProgress(GameItem item)
	{
		if (!IsComplete)
		{
			Data.Progress++;
		}
		onUpdate.OnNext((IObjective)this);
	}
}
