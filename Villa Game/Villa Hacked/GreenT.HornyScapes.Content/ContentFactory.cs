using System;
using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.Booster;
using GreenT.HornyScapes.Characters.Skins.Content;
using GreenT.HornyScapes.Meta.Decorations;
using GreenT.HornyScapes.Presents.Models;
using StripClub.Model;
using Zenject;

namespace GreenT.HornyScapes.Content;

public class ContentFactory : IContentFactory
{
	private readonly CurrencyContentFactory currencyFactory;

	private readonly IFactory<int, LinkedContentAnalyticData, LootboxLinkedContent> lootboxContentFactory;

	private readonly IFactory<int, int, int, LinkedContentAnalyticData, CardLinkedContent> cardContentFactory;

	private readonly IFactory<int, int, LinkedContentAnalyticData, MergeItemLinkedContent> mergeContentFactory;

	private readonly IFactory<int, LinkedContentAnalyticData, SkinLinkedContent> skinContentFactory;

	private readonly IFactory<int, LinkedContentAnalyticData, BoosterLinkedContent> _boosterContentFactory;

	private readonly IFactory<int, LinkedContentAnalyticData, DecorationLinkedContent> decorationFactory;

	private readonly IFactory<string, int, LinkedContentAnalyticData, PresentLinkedContent> _presentFactory;

	public ContentFactory(CurrencyContentFactory currencyFactory, IFactory<int, LinkedContentAnalyticData, LootboxLinkedContent> lootboxContentFactory, IFactory<int, int, int, LinkedContentAnalyticData, CardLinkedContent> cardContentFactory, IFactory<int, int, LinkedContentAnalyticData, MergeItemLinkedContent> mergeContentFactory, IFactory<int, LinkedContentAnalyticData, SkinLinkedContent> skinContentFactory, IFactory<int, LinkedContentAnalyticData, BoosterLinkedContent> boosterContentFactory, IFactory<int, LinkedContentAnalyticData, DecorationLinkedContent> decorationFactory, IFactory<string, int, LinkedContentAnalyticData, PresentLinkedContent> presentFactory)
	{
		this.currencyFactory = currencyFactory;
		this.lootboxContentFactory = lootboxContentFactory;
		this.cardContentFactory = cardContentFactory;
		this.mergeContentFactory = mergeContentFactory;
		this.skinContentFactory = skinContentFactory;
		_boosterContentFactory = boosterContentFactory;
		this.decorationFactory = decorationFactory;
		_presentFactory = presentFactory;
	}

	public LinkedContent Create(LinkedContent.Map map)
	{
		if (!(map is BoosterLinkedContent.BoosterMap boosterMap))
		{
			if (!(map is CardLinkedContent.CardMap cardMap))
			{
				if (!(map is CurrencyLinkedContent.CurrencyMap currencyMap))
				{
					if (!(map is BattlePassLevelLinkedContent.BattlePassLevelMap battlePassLevelMap))
					{
						if (!(map is LootboxLinkedContent.LootboxMap lootboxMap))
						{
							if (!(map is MergeItemLinkedContent.MergeItemMapper mergeItemMapper))
							{
								if (!(map is SkinLinkedContent.SkinMap skinMap))
								{
									if (!(map is DecorationLinkedContent.DecorationMap decorationMap))
									{
										if (map is PresentLinkedContent.PresentMap presentMap)
										{
											return _presentFactory.Create(presentMap.ID, presentMap.Quantity, presentMap.AnalyticData);
										}
										throw new ArgumentOutOfRangeException(map.GetType().ToString()).LogException();
									}
									return decorationFactory.Create(decorationMap.ID, decorationMap.AnalyticData);
								}
								return skinContentFactory.Create(skinMap.ID, skinMap.AnalyticData);
							}
							return mergeContentFactory.Create(mergeItemMapper.Id, mergeItemMapper.Quantity, mergeItemMapper.AnalyticData);
						}
						return lootboxContentFactory.Create(lootboxMap.Id, lootboxMap.AnalyticData);
					}
					return currencyFactory.Create(battlePassLevelMap.Quantity, battlePassLevelMap.Type, battlePassLevelMap.AnalyticData, battlePassLevelMap.CompositeIdentificator);
				}
				return currencyFactory.Create(currencyMap.Quantity, currencyMap.Type, currencyMap.AnalyticData, currencyMap.CompositeIdentificator);
			}
			return cardContentFactory.Create(cardMap.Id, cardMap.GroupId, cardMap.Count, cardMap.AnalyticData);
		}
		return _boosterContentFactory.Create(boosterMap.ID, boosterMap.AnalyticData);
	}
}
