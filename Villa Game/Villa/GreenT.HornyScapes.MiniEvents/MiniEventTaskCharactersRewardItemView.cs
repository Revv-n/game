using System;
using GreenT.HornyScapes.Characters.Skins.Content;
using StripClub.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class MiniEventTaskCharactersRewardItemView : MiniEventTaskRewardItemView
{
	[SerializeField]
	private Image _icon;

	[SerializeField]
	private TMP_Text _amount;

	public int GirlID { get; private set; }

	public int SkinID { get; private set; }

	public override void Set(LinkedContent source)
	{
		base.Set(source);
		if (base.Source is CardLinkedContent cardLinkedContent)
		{
			_icon.sprite = cardLinkedContent.GetSquareIcon();
			_backgroundFrame.Set((int)cardLinkedContent.GetRarity());
			_amount.text = $"{cardLinkedContent.Quantity}";
			SkinID = 0;
			GirlID = cardLinkedContent.Card.ID;
			return;
		}
		if (base.Source is SkinLinkedContent skinLinkedContent)
		{
			_icon.sprite = skinLinkedContent.GetSquareIcon();
			_backgroundFrame.Set((int)skinLinkedContent.GetRarity());
			_amount.text = $"{skinLinkedContent.Count()}";
			GirlID = 0;
			SkinID = skinLinkedContent.Skin.ID;
			return;
		}
		throw new Exception().SendException($"{GetType().Name}: You're trying to display contet of type: {base.Source.GetType()} inside {GetType().Name} ! ");
	}
}
