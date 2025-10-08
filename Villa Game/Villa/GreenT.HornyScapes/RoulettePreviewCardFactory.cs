using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Booster;
using GreenT.HornyScapes.Characters.Skins.Content;
using GreenT.HornyScapes.Content;
using GreenT.HornyScapes.Meta.Decorations;
using Merge;
using StripClub.Model;
using Zenject;

namespace GreenT.HornyScapes;

public class RoulettePreviewCardFactory : IFactory<LinkedContent, bool, PromoCardView>, IFactory
{
	private Dictionary<(Type, bool), IFactory<LinkedContent, PromoCardView>> _dictionary;

	public RoulettePreviewCardFactory(IFactory<LinkedContent, SmallGirlPromoCardView> smallGirlFactory, IFactory<LinkedContent, SmallAnyPromoCardView> smallAnyFactory, IFactory<LinkedContent, BigGirlPromoCardView> bigGirlFactory, IFactory<LinkedContent, BigAnyPromoCardView> bigAnyFactory)
	{
		_dictionary = new Dictionary<(Type, bool), IFactory<LinkedContent, PromoCardView>>
		{
			{
				(typeof(CardLinkedContent), false),
				smallGirlFactory
			},
			{
				(typeof(CardLinkedContent), true),
				bigGirlFactory
			},
			{
				(typeof(CurrencyLinkedContent), false),
				smallAnyFactory
			},
			{
				(typeof(CurrencyLinkedContent), true),
				bigAnyFactory
			},
			{
				(typeof(CurrencySpecialLinkedContent), false),
				smallAnyFactory
			},
			{
				(typeof(CurrencySpecialLinkedContent), true),
				bigAnyFactory
			},
			{
				(typeof(MergeItemLinkedContent), false),
				smallAnyFactory
			},
			{
				(typeof(MergeItemLinkedContent), true),
				bigAnyFactory
			},
			{
				(typeof(SkinLinkedContent), false),
				smallGirlFactory
			},
			{
				(typeof(SkinLinkedContent), true),
				bigGirlFactory
			},
			{
				(typeof(DecorationLinkedContent), false),
				smallAnyFactory
			},
			{
				(typeof(DecorationLinkedContent), true),
				bigAnyFactory
			},
			{
				(typeof(LootboxLinkedContent), false),
				smallAnyFactory
			},
			{
				(typeof(LootboxLinkedContent), true),
				bigAnyFactory
			},
			{
				(typeof(BattlePassLevelLinkedContent), false),
				smallAnyFactory
			},
			{
				(typeof(BattlePassLevelLinkedContent), true),
				bigAnyFactory
			},
			{
				(typeof(BoosterLinkedContent), false),
				smallAnyFactory
			},
			{
				(typeof(BoosterLinkedContent), true),
				bigAnyFactory
			}
		};
	}

	public virtual PromoCardView Create(LinkedContent source, bool isBig)
	{
		if (source == null)
		{
			throw new Exception().SendException(GetType().Name + ": Reward content is empty: " + source);
		}
		Type type = source.GetType();
		if (_dictionary.TryGetValue((type, isBig), out var value))
		{
			PromoCardView promoCardView = value.Create(source);
			promoCardView.SetActive(active: true);
			return promoCardView;
		}
		throw new Exception().SendException($"{GetType().Name}: no behavior for this content type: {type}");
	}
}
