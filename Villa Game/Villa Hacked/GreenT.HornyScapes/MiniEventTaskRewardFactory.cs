using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Characters.Skins.Content;
using GreenT.HornyScapes.Content;
using GreenT.HornyScapes.Meta.Decorations;
using GreenT.HornyScapes.MiniEvents;
using Merge;
using StripClub.Model;
using Zenject;

namespace GreenT.HornyScapes;

public sealed class MiniEventTaskRewardFactory : IFactory<LinkedContent, MiniEventTaskRewardItemView>, IFactory
{
	private Dictionary<Type, IFactory<LinkedContent, MiniEventTaskRewardItemView>> _dictionary;

	public MiniEventTaskRewardFactory(IFactory<LinkedContent, MiniEventTaskLootboxRewardItemView> lootboxFactory, IFactory<LinkedContent, MiniEventTaskCurrencyRewardItemView> currencyFactory, IFactory<LinkedContent, MiniEventTaskCharactersRewardItemView> charactersFactory, IFactory<LinkedContent, MiniEventTaskDecorationRewardItemView> decorationsFactory, IFactory<LinkedContent, MiniEventTaskMergeRewardItemView> mergeItemsFactory)
	{
		_dictionary = new Dictionary<Type, IFactory<LinkedContent, MiniEventTaskRewardItemView>>
		{
			{
				typeof(LootboxLinkedContent),
				(IFactory<LinkedContent, MiniEventTaskRewardItemView>)(object)lootboxFactory
			},
			{
				typeof(CurrencyLinkedContent),
				(IFactory<LinkedContent, MiniEventTaskRewardItemView>)(object)currencyFactory
			},
			{
				typeof(CardLinkedContent),
				(IFactory<LinkedContent, MiniEventTaskRewardItemView>)(object)charactersFactory
			},
			{
				typeof(SkinLinkedContent),
				(IFactory<LinkedContent, MiniEventTaskRewardItemView>)(object)charactersFactory
			},
			{
				typeof(DecorationLinkedContent),
				(IFactory<LinkedContent, MiniEventTaskRewardItemView>)(object)decorationsFactory
			},
			{
				typeof(MergeItemLinkedContent),
				(IFactory<LinkedContent, MiniEventTaskRewardItemView>)(object)mergeItemsFactory
			}
		};
	}

	public MiniEventTaskRewardItemView Create(LinkedContent source)
	{
		if (source == null)
		{
			throw new Exception().SendException(GetType().Name + ": Reward is empty: " + source);
		}
		Type type = source.GetType();
		if (_dictionary.TryGetValue(type, out var value))
		{
			MiniEventTaskRewardItemView miniEventTaskRewardItemView = value.Create(source);
			miniEventTaskRewardItemView.SetActive(active: true);
			return miniEventTaskRewardItemView;
		}
		throw new Exception().SendException($"{GetType().Name}: no behavior for this content type: {type}");
	}
}
