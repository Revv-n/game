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
				girlFactory
			},
			{
				typeof(CurrencyLinkedContent),
				itemFactory
			},
			{
				typeof(CurrencySpecialLinkedContent),
				itemFactory
			},
			{
				typeof(MergeItemLinkedContent),
				itemFactory
			},
			{
				typeof(SkinLinkedContent),
				skinFactory
			},
			{
				typeof(DecorationLinkedContent),
				itemFactory
			},
			{
				typeof(BattlePassLevelLinkedContent),
				itemFactory
			}
		};
	}

	public virtual EventRewardCard Create(EventReward source)
	{
		if (source.Content == null)
		{
			Exception innerException = new Exception();
			string name = GetType().Name;
			int target = source.Target;
			throw innerException.SendException(name + ": Reward content is empty: " + target);
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
