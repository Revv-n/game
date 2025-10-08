using System;
using GreenT.HornyScapes.UI;
using StripClub.Model.Cards;
using TMPro;
using UnityEngine;

namespace StripClub.Rewards.UI;

public class RewardCardView : ProgressCardView
{
	[Serializable]
	public struct Settings
	{
		public Color Glow;
	}

	[Serializable]
	public class SettingsDictionary : SerializableDictionary<Rarity, Settings>
	{
	}

	[SerializeField]
	private GameObject newLabel;

	[SerializeField]
	private GameObject cardBack;

	[SerializeField]
	private TMP_Text quantityText;

	public override void Set(ICard card)
	{
		base.Set(card);
		SetNewLabel(base.promote.IsNew.Value);
		cardBack.SetActive(value: false);
	}

	public void SetQuantityText(string text)
	{
		quantityText.text = text;
	}

	public void SetNewLabel(bool value)
	{
		newLabel.SetActive(value);
	}
}
