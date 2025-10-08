using System;
using GreenT.HornyScapes.UI;
using StripClub.Model.Cards;
using StripClub.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.MergeCore.UI;

public class CharacterBonusView : CardView
{
	[SerializeField]
	private Image characterIcon;

	[SerializeField]
	private TMP_Text girlLevel;

	[SerializeField]
	private TMP_Text bonusValue;

	[SerializeField]
	private Image bonusIcon;

	[SerializeField]
	private StatableComponent[] statableComponents;

	private IDisposable disposable;

	private IPromote promote;

	private GameSettings gameSettings;

	private CardsCollection cards;

	[Inject]
	private void InnerInit(GameSettings gameSettings, CardsCollection cards)
	{
		this.gameSettings = gameSettings;
		this.cards = cards;
	}

	public override void Set(ICard source)
	{
		disposable?.Dispose();
		base.Set(source);
		promote = cards.GetPromoteOrDefault(source);
		SetGirlInfo(source);
		SetBonus(source);
	}

	private void SetGirlInfo(ICard card)
	{
		girlLevel.text = promote.Level.Value.ToString();
	}

	private void SetBonus(ICard card)
	{
		bonusIcon.sprite = gameSettings.BonusSettings[card.Bonus.BonusType].BonusSprite;
	}

	private void SetStatable(int progress)
	{
		StatableComponent[] array = statableComponents;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Set((progress >= promote.Target) ? 1 : 0);
		}
	}
}
