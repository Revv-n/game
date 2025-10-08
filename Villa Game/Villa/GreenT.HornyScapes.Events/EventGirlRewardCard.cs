using System;
using System.Collections.Generic;
using StripClub.Model;
using StripClub.Model.Cards;
using StripClub.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Events;

public class EventGirlRewardCard : EventRewardCard
{
	[SerializeField]
	protected Image icon;

	[SerializeField]
	protected List<StatableComponent> rarityComponenets;

	public override void Set(EventReward source)
	{
		base.Set(source);
		if (source.Content is CardLinkedContent cardLinkedContent)
		{
			icon.sprite = cardLinkedContent.GetIcon();
			SetRarity(cardLinkedContent.Card.Rarity);
			return;
		}
		throw new Exception().SendException($"{GetType().Name}: You're trying to display contet of type: {source.Content.GetType()} inside {GetType().Name} ! ");
	}

	private void SetRarity(Rarity rarity)
	{
		foreach (StatableComponent rarityComponenet in rarityComponenets)
		{
			rarityComponenet.Set((int)rarity);
		}
	}
}
