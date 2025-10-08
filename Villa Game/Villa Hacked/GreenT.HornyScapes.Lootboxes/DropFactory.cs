using System;
using GreenT.HornyScapes.Analytics;
using GreenT.Types;
using ModestTree;
using StripClub.Model;
using StripClub.Model.Cards;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Lootboxes;

public class DropFactory : IFactory<RewType, Selector, int, int, ContentType, DropSettings>, IFactory, IFactory<RewType, Selector, int, int, ContentType, int, RandomDropSettings>
{
	private readonly CardsCollection cards;

	private readonly LinkedContentAnalyticDataFactory analyticDataFactory;

	private readonly IFactory<RewType, Selector, int, int, ContentType, LinkedContentAnalyticData, LinkedContent> linkedContentFactory;

	public DropFactory(CardsCollection cards, LinkedContentAnalyticDataFactory analyticDataFactory, IFactory<RewType, Selector, int, int, ContentType, LinkedContentAnalyticData, LinkedContent> linkedContentFactory)
	{
		Assert.IsNotNull((object)cards);
		this.cards = cards;
		this.analyticDataFactory = analyticDataFactory;
		this.linkedContentFactory = linkedContentFactory;
	}

	public DropSettings Create(RewType type, Selector selector, int quantity, int delta, ContentType contentType)
	{
		if (Mathf.Abs(quantity) < Mathf.Abs(delta))
		{
			throw new ArgumentOutOfRangeException("Game Design Error! Quantity can't be lesser then Delta. Quantity:" + quantity + " Delta:" + delta);
		}
		return new DropSettings(type, selector, quantity, delta, contentType, cards, analyticDataFactory, linkedContentFactory);
	}

	public RandomDropSettings Create(RewType type, Selector selector, int quantity, int delta, ContentType contentType, int weight)
	{
		return new RandomDropSettings(type, selector, quantity, delta, contentType, weight, cards, analyticDataFactory, linkedContentFactory);
	}
}
