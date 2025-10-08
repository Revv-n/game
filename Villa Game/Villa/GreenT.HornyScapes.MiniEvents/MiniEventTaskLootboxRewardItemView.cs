using System;
using StripClub.Model;
using StripClub.Model.Cards;
using TMPro;
using UnityEngine;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class MiniEventTaskLootboxRewardItemView : MiniEventTaskRewardItemView
{
	[SerializeField]
	private TMP_Text _count;

	public Rarity Rarity { get; private set; }

	public override void Set(LinkedContent source)
	{
		base.Set(source);
		if (base.Source is LootboxLinkedContent lootboxLinkedContent)
		{
			_rewardIcon.Set((int)lootboxLinkedContent.Lootbox.Rarity);
			_backgroundFrame.Set((int)lootboxLinkedContent.Lootbox.Rarity);
			Rarity = lootboxLinkedContent.Lootbox.Rarity;
			_count.text = $"{base.Source.Count()}";
			return;
		}
		throw new Exception().SendException($"{GetType().Name}: You're trying to display contet of type: {base.Source.GetType()} inside {GetType().Name} ! ");
	}
}
