using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Characters.Skins.Content;
using GreenT.HornyScapes.Events;
using StripClub.Model.Cards;
using StripClub.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Characters.Skins.Events;

public class EventSkinRewardCard : EventRewardCard
{
	[SerializeField]
	protected Image icon;

	[SerializeField]
	protected List<StatableComponent> rarityComponenets;

	public override void Set(EventReward source)
	{
		base.Set(source);
		if (source.Content is SkinLinkedContent skinLinkedContent)
		{
			icon.sprite = skinLinkedContent.GetIcon();
			SetRarity(skinLinkedContent.Skin.Rarity);
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
