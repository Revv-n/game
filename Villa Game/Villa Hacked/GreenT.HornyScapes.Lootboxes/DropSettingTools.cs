using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.Types;
using GreenT.Utilities;
using StripClub.Model.Cards;
using UnityEngine;

namespace GreenT.HornyScapes.Lootboxes;

public static class DropSettingTools
{
	public static T GetCardBySelectorOrDefault<T>(CardsCollection cards, Selector selector, ContentType contentType) where T : class, ICard
	{
		T[] array = cards.Collection.OfType<T>().ToArray();
		if (TryGetCardByIDSelector(selector, array, out var card))
		{
			return card;
		}
		TryGetCardByCardSelector(cards, selector, array, contentType, out card);
		return card;
	}

	private static bool TryGetCardByIDSelector<T>(Selector selector, IEnumerable<T> cardsOfNecessaryType, out T card) where T : class, ICard
	{
		card = null;
		if (selector is SelectorByID selectorByID)
		{
			int id = selectorByID.ID;
			try
			{
				card = cardsOfNecessaryType.First((T _card) => _card.ID == id);
			}
			catch (Exception innerException)
			{
				throw innerException.SendException("В коллекции отсутсвует карточка с ID: " + id + " типа " + typeof(T));
			}
		}
		return card != null;
	}

	private static bool TryGetCardByCardSelector<T>(CardsCollection cards, Selector selector, IEnumerable<T> cardsOfNeccesaryType, ContentType contentType, out T card) where T : class, ICard
	{
		card = null;
		if (selector is CardSelector cardSelector)
		{
			if (cardSelector.Pool == CardSelector.TargetPool.In)
			{
				card = GetCardByTargetPoolIn(cards, cardSelector, cardsOfNeccesaryType, contentType);
			}
			else if (cardSelector.Pool == CardSelector.TargetPool.Out)
			{
				card = GetCardByTargetPoolOut(cards, cardSelector, cardsOfNeccesaryType, contentType);
			}
		}
		return card != null;
	}

	private static T GetCardByTargetPoolIn<T>(CardsCollection cards, CardSelector cardSelector, IEnumerable<T> cardsOfNecessaryType, ContentType contentType) where T : class, ICard
	{
		T result = null;
		if (cardSelector.Pool == CardSelector.TargetPool.In)
		{
			IPromote[] promotes = (from _card in cardsOfNecessaryType.Where(HasPromote)
				select cards.Promote[_card]).ToArray();
			if (promotes.Length != 0)
			{
				int id = 0;
				if (promotes.Length > 1)
				{
					int[] levels = promotes.Select((IPromote _promote) => _promote.Level.Value).ToArray();
					id = GreenT.Utilities.Random.GetRandomElementByLevel(levels);
				}
				return (T)cards.Promote.First((System.Collections.Generic.KeyValuePair<ICard, IPromote> _pair) => _pair.Value == promotes[id]).Key;
			}
		}
		return result;
		bool HasPromote(T _card)
		{
			if (cards.Promote.ContainsKey(_card) && _card.Rarity == cardSelector.Rarity)
			{
				return _card.ContentType == contentType;
			}
			return false;
		}
	}

	private static T GetCardByTargetPoolOut<T>(CardsCollection cards, CardSelector cardSelector, IEnumerable<T> cardsOfNecessaryType, ContentType contentType) where T : class, ICard
	{
		T result = null;
		T[] array = cardsOfNecessaryType.Where(delegate(T _character)
		{
			IPromote promoteOrDefault = cards.GetPromoteOrDefault(_character);
			return _character.Rarity == cardSelector.Rarity && promoteOrDefault == null && _character.ContentType == contentType;
		}).ToArray();
		if (array.Any())
		{
			int num = UnityEngine.Random.Range(0, array.Length);
			return array[num];
		}
		return result;
	}
}
