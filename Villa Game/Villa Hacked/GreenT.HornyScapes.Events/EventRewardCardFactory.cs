using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Characters.Skins.Content;
using GreenT.HornyScapes.Characters.Skins.Events;
using GreenT.HornyScapes.Content;
using GreenT.HornyScapes.Meta.Decorations;
using Merge;
using StripClub.Model;
using Zenject;

namespace GreenT.HornyScapes.Events;

public class EventRewardCardFactory : IFactory<EventReward, EventRewardCard>, IFactory
{
	private readonly Dictionary<Type, IFactory<EventReward, EventRewardCard>> dictionary;

	public EventRewardCardFactory(IFactory<EventReward, EventGirlRewardCard> girlFactory, IFactory<EventReward, EventItemsRewardCard> itemFactory, IFactory<EventReward, EventSkinRewardCard> skinFactory)
	{
		dictionary = new Dictionary<Type, IFactory<EventReward, EventRewardCard>>
		{
			{
				typeof(CardLinkedContent),
				(IFactory<EventReward, EventRewardCard>)(object)girlFactory
			},
			{
				typeof(CurrencyLinkedContent),
				(IFactory<EventReward, EventRewardCard>)(object)itemFactory
			},
			{
				typeof(CurrencySpecialLinkedContent),
				(IFactory<EventReward, EventRewardCard>)(object)itemFactory
			},
			{
				typeof(MergeItemLinkedContent),
				(IFactory<EventReward, EventRewardCard>)(object)itemFactory
			},
			{
				typeof(SkinLinkedContent),
				(IFactory<EventReward, EventRewardCard>)(object)skinFactory
			},
			{
				typeof(DecorationLinkedContent),
				(IFactory<EventReward, EventRewardCard>)(object)itemFactory
			},
			{
				typeof(BattlePassLevelLinkedContent),
				(IFactory<EventReward, EventRewardCard>)(object)itemFactory
			}
		};
	}

	public virtual EventRewardCard Create(EventReward source)
	{
		if (source.Content == null)
		{
			throw new Exception().SendException(GetType().Name + ": Reward content is empty: " + source.Target);
		}
		Type type = source.Content.GetType();
		if (dictionary.TryGetValue(type, out var value))
		{
			EventRewardCard eventRewardCard = value.Create(source);
			eventRewardCard.SetActive(active: true);
			return eventRewardCard;
		}
		throw new Exception().SendException($"{GetType().Name}: no behavior for this content type: {type}");
	}
}
