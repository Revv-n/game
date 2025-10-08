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
				(IFactory<LinkedContent, PromoCardView>)(object)smallGirlFactory
			},
			{
				(typeof(CardLinkedContent), true),
				(IFactory<LinkedContent, PromoCardView>)(object)bigGirlFactory
			},
			{
				(typeof(CurrencyLinkedContent), false),
				(IFactory<LinkedContent, PromoCardView>)(object)smallAnyFactory
			},
			{
				(typeof(CurrencyLinkedContent), true),
				(IFactory<LinkedContent, PromoCardView>)(object)bigAnyFactory
			},
			{
				(typeof(CurrencySpecialLinkedContent), false),
				(IFactory<LinkedContent, PromoCardView>)(object)smallAnyFactory
			},
			{
				(typeof(CurrencySpecialLinkedContent), true),
				(IFactory<LinkedContent, PromoCardView>)(object)bigAnyFactory
			},
			{
				(typeof(MergeItemLinkedContent), false),
				(IFactory<LinkedContent, PromoCardView>)(object)smallAnyFactory
			},
			{
				(typeof(MergeItemLinkedContent), true),
				(IFactory<LinkedContent, PromoCardView>)(object)bigAnyFactory
			},
			{
				(typeof(SkinLinkedContent), false),
				(IFactory<LinkedContent, PromoCardView>)(object)smallGirlFactory
			},
			{
				(typeof(SkinLinkedContent), true),
				(IFactory<LinkedContent, PromoCardView>)(object)bigGirlFactory
			},
			{
				(typeof(DecorationLinkedContent), false),
				(IFactory<LinkedContent, PromoCardView>)(object)smallAnyFactory
			},
			{
				(typeof(DecorationLinkedContent), true),
				(IFactory<LinkedContent, PromoCardView>)(object)bigAnyFactory
			},
			{
				(typeof(LootboxLinkedContent), false),
				(IFactory<LinkedContent, PromoCardView>)(object)smallAnyFactory
			},
			{
				(typeof(LootboxLinkedContent), true),
				(IFactory<LinkedContent, PromoCardView>)(object)bigAnyFactory
			},
			{
				(typeof(BattlePassLevelLinkedContent), false),
				(IFactory<LinkedContent, PromoCardView>)(object)smallAnyFactory
			},
			{
				(typeof(BattlePassLevelLinkedContent), true),
				(IFactory<LinkedContent, PromoCardView>)(object)bigAnyFactory
			},
			{
				(typeof(BoosterLinkedContent), false),
				(IFactory<LinkedContent, PromoCardView>)(object)smallAnyFactory
			},
			{
				(typeof(BoosterLinkedContent), true),
				(IFactory<LinkedContent, PromoCardView>)(object)bigAnyFactory
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
