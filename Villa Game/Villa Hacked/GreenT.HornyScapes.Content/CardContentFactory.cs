using System.Linq;
using GreenT.HornyScapes.Analytics;
using StripClub.Model;
using StripClub.Model.Cards;
using Zenject;

namespace GreenT.HornyScapes.Content;

public class CardContentFactory : IFactory<int, int, int, LinkedContentAnalyticData, CardLinkedContent>, IFactory, IFactory<int, int, int, LinkedContentAnalyticData, LinkedContent, CardLinkedContent>, IFactory<ICard, int, LinkedContentAnalyticData, LinkedContent, CardLinkedContent>
{
	private readonly CardsCollection cards;

	public CardContentFactory(CardsCollection cardsCollection)
	{
		cards = cardsCollection;
	}

	public CardLinkedContent Create(int cardID, int groupID, int count, LinkedContentAnalyticData analyticData, LinkedContent nestedContent = null)
	{
		ICard card = cards.Collection.First((ICard _card) => _card.ID == cardID && _card.GroupID == groupID);
		return Create(card, count, analyticData, nestedContent);
	}

	public CardLinkedContent Create(int cardID, int groupID, int count, LinkedContentAnalyticData analyticData)
	{
		return Create(cardID, groupID, count, analyticData, null);
	}

	public CardLinkedContent Create(ICard card, int quantity, LinkedContentAnalyticData analyticData, LinkedContent nestedContent = null)
	{
		return new CardLinkedContent(card, quantity, cards, analyticData, nestedContent);
	}
}
