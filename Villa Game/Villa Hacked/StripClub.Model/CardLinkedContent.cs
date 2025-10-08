using System;
using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.Characters;
using GreenT.HornyScapes.Exceptions;
using StripClub.Model.Cards;
using UnityEngine;

namespace StripClub.Model;

public class CardLinkedContent : LinkedContent
{
	[Serializable]
	public class CardMap : Map
	{
		[SerializeField]
		private int id;

		[SerializeField]
		private int groupId;

		[SerializeField]
		private int count;

		public int Id => id;

		public int GroupId => groupId;

		public int Count => count;

		public CardMap(CardLinkedContent source)
			: base(source)
		{
			id = source.Card.ID;
			groupId = source.Card.GroupID;
			count = source.Quantity;
		}
	}

	private readonly CardsCollection collection;

	private Sprite icon;

	public ICard Card { get; }

	public int Quantity { get; }

	public override Type Type => typeof(CardLinkedContent);

	public CardLinkedContent(ICard card, int quantity, CardsCollection collection, LinkedContentAnalyticData analyticData, LinkedContent next = null)
		: base(analyticData, next)
	{
		Card = card;
		Quantity = quantity;
		this.collection = collection;
	}

	public override Rarity GetRarity()
	{
		return Card.Rarity;
	}

	public override string GetName()
	{
		return Card.NameKey;
	}

	public override string GetDescription()
	{
		return Quantity.ToString();
	}

	public override Sprite GetIcon()
	{
		if (icon != null)
		{
			return icon;
		}
		if (!(Card is ICharacter character))
		{
			throw new TypeMismatchException(Card.GetType(), typeof(ICharacter));
		}
		icon = character.GetBundleData().GetLevelAvatar(1);
		return icon;
	}

	public Sprite GetSquareIcon()
	{
		if (icon != null)
		{
			return icon;
		}
		if (!(Card is ICharacter character))
		{
			throw new TypeMismatchException(Card.GetType(), typeof(ICharacter));
		}
		icon = character.GetBundleData().SquareIcon;
		return icon;
	}

	public Sprite GetFullIcon()
	{
		if (icon != null)
		{
			return icon;
		}
		if (!(Card is ICharacter character))
		{
			throw new TypeMismatchException(Card.GetType(), typeof(ICharacter));
		}
		icon = character.GetBundleData().DefaultAvatar;
		return icon;
	}

	public override Sprite GetProgressBarIcon()
	{
		return Card.ProgressBarIcon;
	}

	public override Map GetMap()
	{
		return new CardMap(this);
	}

	public override void AddCurrentToPlayer()
	{
		collection.AddSouls(Card, Quantity);
		base.AddCurrentToPlayer();
	}

	public override LinkedContent Clone()
	{
		return new CardLinkedContent(Card, Quantity, collection, AnalyticData, base.CloneOfNext);
	}
}
