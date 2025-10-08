using System;
using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.Characters;
using GreenT.Types;
using StripClub.Model;
using StripClub.Model.Cards;
using StripClub.Model.Lootboxes;
using Zenject;

namespace GreenT.HornyScapes.Lootboxes;

public class DropSettings : IDrop
{
	public readonly ContentType ContentType;

	private CardsCollection cards;

	private readonly LinkedContentAnalyticDataFactory analyticDataFactory;

	private readonly IFactory<RewType, Selector, int, int, ContentType, LinkedContentAnalyticData, LinkedContent> linkedContentFactory;

	public RewType Type { get; }

	public Selector Selector { get; }

	public int Quantity { get; }

	public int Delta { get; }

	public DropSettings(RewType type, Selector selector, int quantity, int delta, ContentType contentType, CardsCollection cards, LinkedContentAnalyticDataFactory analyticDataFactory, IFactory<RewType, Selector, int, int, ContentType, LinkedContentAnalyticData, LinkedContent> linkedContentFactory)
	{
		Type = type;
		Quantity = quantity;
		Delta = delta;
		Selector = selector;
		ContentType = contentType;
		this.cards = cards;
		this.analyticDataFactory = analyticDataFactory;
		this.linkedContentFactory = linkedContentFactory;
	}

	public bool IsAvailable()
	{
		switch (Type)
		{
		case RewType.Characters:
			return DropSettingTools.GetCardBySelectorOrDefault<ICharacter>(cards, Selector, ContentType) != null;
		case RewType.Decorations:
		case RewType.Resource:
		case RewType.MergeItem:
		case RewType.Lootbox:
		case RewType.Skin:
		case RewType.Level:
		case RewType.Booster:
			return true;
		default:
			throw new NotImplementedException("There is no behaviour for this rew type:" + Type);
		}
	}

	public virtual LinkedContent GetDrop(CurrencyAmplitudeAnalytic.SourceType sourceType)
	{
		LinkedContentAnalyticData linkedContentAnalyticData = analyticDataFactory.Create(sourceType);
		return linkedContentFactory.Create(Type, Selector, Quantity, Delta, ContentType, linkedContentAnalyticData);
	}
}
