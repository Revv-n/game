using System;
using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.Booster;
using GreenT.HornyScapes.Characters;
using GreenT.HornyScapes.Characters.Skins.Content;
using GreenT.HornyScapes.Content;
using GreenT.HornyScapes.Dates.Models;
using GreenT.HornyScapes.Meta.Decorations;
using GreenT.Types;
using StripClub.Model;
using StripClub.Model.Cards;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Lootboxes;

public class LinkedContentFactory : IFactory<RewType, Selector, int, int, ContentType, LinkedContentAnalyticData, LinkedContent>, IFactory
{
	private readonly CardsCollection cards;

	private readonly CurrencyContentFactory currencyFactory;

	private readonly IFactory<ICard, int, LinkedContentAnalyticData, LinkedContent, CardLinkedContent> cardFactory;

	private readonly IFactory<int, LinkedContentAnalyticData, LinkedContent, SkinLinkedContent> skinFactory;

	private readonly IFactory<int, LinkedContentAnalyticData, BattlePassLevelLinkedContent> battlePassLevelFactory;

	private readonly IFactory<int, LinkedContentAnalyticData, LootboxLinkedContent> lootboxContentFactory;

	private readonly IFactory<int, int, LinkedContentAnalyticData, MergeItemLinkedContent> mergeItemContentFactory;

	private readonly IFactory<int, LinkedContentAnalyticData, BoosterLinkedContent> _boosterContentFactory;

	private readonly IFactory<int, LinkedContentAnalyticData, DecorationLinkedContent> decorationFactory;

	private readonly IFactory<int, LinkedContentAnalyticData, LinkedContent, DateLinkedContent> _dateContentFactory;

	private readonly IFactory<int, LinkedContentAnalyticData, LinkedContent, ComingSoonDateLinkedContent> _comingSoonDateContentFactory;

	public LinkedContentFactory(CardsCollection cards, CurrencyContentFactory currencyFactory, IFactory<int, LinkedContentAnalyticData, BattlePassLevelLinkedContent> battlePassLevelFactory, IFactory<int, LinkedContentAnalyticData, LootboxLinkedContent> lootboxContentFactory, IFactory<int, int, LinkedContentAnalyticData, MergeItemLinkedContent> mergeItemContentFactory, IFactory<ICard, int, LinkedContentAnalyticData, LinkedContent, CardLinkedContent> cardFactory, IFactory<int, LinkedContentAnalyticData, LinkedContent, SkinLinkedContent> skinFactory, IFactory<int, LinkedContentAnalyticData, DecorationLinkedContent> decorationFactory, IFactory<int, LinkedContentAnalyticData, BoosterLinkedContent> boosterContentFactory, IFactory<int, LinkedContentAnalyticData, LinkedContent, DateLinkedContent> dateContentFactory, IFactory<int, LinkedContentAnalyticData, LinkedContent, ComingSoonDateLinkedContent> comingSoonDateContentFactory)
	{
		this.cards = cards;
		this.cardFactory = cardFactory;
		this.skinFactory = skinFactory;
		this.currencyFactory = currencyFactory;
		this.battlePassLevelFactory = battlePassLevelFactory;
		this.lootboxContentFactory = lootboxContentFactory;
		this.mergeItemContentFactory = mergeItemContentFactory;
		this.decorationFactory = decorationFactory;
		_boosterContentFactory = boosterContentFactory;
		_dateContentFactory = dateContentFactory;
		_comingSoonDateContentFactory = comingSoonDateContentFactory;
	}

	public LinkedContent Create(RewType rewType, Selector selector, int quantity, int delta, ContentType contentType, LinkedContentAnalyticData analyticData)
	{
		int dropQuantity = quantity + UnityEngine.Random.Range(-delta, delta + 1);
		return rewType switch
		{
			RewType.Characters => GetCard(selector, contentType, analyticData, dropQuantity), 
			RewType.Level => GetBattlePassLevel(selector, analyticData, dropQuantity), 
			RewType.Resource => GetResource(selector, analyticData, dropQuantity), 
			RewType.MergeItem => GetMergeItem(selector, analyticData, dropQuantity), 
			RewType.Lootbox => GetLootBox(selector, analyticData), 
			RewType.Skin => GetSkin(selector, analyticData), 
			RewType.Decorations => GetDecorationLinkedContent(selector, analyticData), 
			RewType.Booster => GetBooster(selector, analyticData), 
			RewType.Date => GetDateLinkedContent(selector, analyticData), 
			RewType.ComingSoonDate => GetComingSoonDateLinkedContent(selector, analyticData), 
			_ => throw new NotImplementedException("There is no behaviour for this rew type:" + rewType), 
		};
	}

	private LinkedContent GetBattlePassLevel(Selector selector, LinkedContentAnalyticData analyticData, int dropQuantity)
	{
		LevelSelector levelSelector = (LevelSelector)selector;
		if (levelSelector.Level == LevelType.BattlePass)
		{
			return battlePassLevelFactory.Create(dropQuantity, analyticData);
		}
		throw new ArgumentOutOfRangeException(string.Format("[{0}] Нет подходящего контейнера для контента уровня - {1}", "LinkedContentFactory", levelSelector.Level));
	}

	private LinkedContent GetCard(Selector selector, ContentType contentType, LinkedContentAnalyticData analyticData, int dropQuantity)
	{
		ICharacter cardBySelectorOrDefault = DropSettingTools.GetCardBySelectorOrDefault<ICharacter>(cards, selector, contentType);
		if (cardBySelectorOrDefault == null)
		{
			return null;
		}
		return cardFactory.Create(cardBySelectorOrDefault, dropQuantity, analyticData, null);
	}

	private LinkedContent GetResource(Selector selector, LinkedContentAnalyticData analyticData, int dropQuantity)
	{
		CurrencySelector currencySelector = (CurrencySelector)selector;
		return currencyFactory.Create(dropQuantity, currencySelector.Currency, analyticData, currencySelector.Identificator);
	}

	private LinkedContent GetMergeItem(Selector selector, LinkedContentAnalyticData analyticData, int dropQuantity)
	{
		int iD = ((SelectorByID)selector).ID;
		return mergeItemContentFactory.Create(iD, dropQuantity, analyticData);
	}

	private LinkedContent GetSkin(Selector selector, LinkedContentAnalyticData analyticData)
	{
		int iD = ((SelectorByID)selector).ID;
		return skinFactory.Create(iD, analyticData, null);
	}

	private LinkedContent GetBooster(Selector selector, LinkedContentAnalyticData analyticData)
	{
		int iD = ((SelectorByID)selector).ID;
		return _boosterContentFactory.Create(iD, analyticData);
	}

	private LinkedContent GetDecorationLinkedContent(Selector selector, LinkedContentAnalyticData analyticData)
	{
		int iD = ((SelectorByID)selector).ID;
		return decorationFactory.Create(iD, analyticData);
	}

	private LootboxLinkedContent GetLootBox(Selector selector, LinkedContentAnalyticData analyticData)
	{
		if (!(selector is SelectorByID selectorByID))
		{
			throw new Exception().SendException(GetType().Name + ": incorrect LootBox selector");
		}
		return lootboxContentFactory.Create(selectorByID.ID, analyticData);
	}

	private LinkedContent GetDateLinkedContent(Selector selector, LinkedContentAnalyticData analyticData)
	{
		int iD = ((SelectorByID)selector).ID;
		return _dateContentFactory.Create(iD, analyticData, null);
	}

	private LinkedContent GetComingSoonDateLinkedContent(Selector selector, LinkedContentAnalyticData analyticData)
	{
		int iD = ((SelectorByID)selector).ID;
		return _comingSoonDateContentFactory.Create(iD, analyticData, null);
	}
}
