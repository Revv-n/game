using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Booster;
using GreenT.HornyScapes.Characters.Skins.Content;
using GreenT.HornyScapes.Content;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.Lootboxes;
using GreenT.HornyScapes.Meta.Decorations;
using GreenT.HornyScapes.Presents.Models;
using GreenT.HornyScapes.Relationships.Analytics;
using StripClub.Messenger.Data;
using StripClub.Model;
using StripClub.Model.Shop.UI;
using Zenject;

namespace GreenT.HornyScapes.Relationships.Views;

public class LootboxRewardView : BaseRewardView
{
	private SmallCardsViewManager _smallCardsViewManager;

	private MessageDropView.Manager _messageDropViewManager;

	private DialogueConfigMapper.Manager _dialogueConfigMapperManager;

	private RelationshipAnalytic _relationshipAnalytic;

	[Inject]
	private void Init(SmallCardsViewManager smallCardsViewManager, MessageDropView.Manager messageDropViewManager, DialogueConfigMapper.Manager dialogueConfigMapperManager, RelationshipAnalytic relationshipAnalytic)
	{
		_smallCardsViewManager = smallCardsViewManager;
		_messageDropViewManager = messageDropViewManager;
		_dialogueConfigMapperManager = dialogueConfigMapperManager;
		_relationshipAnalytic = relationshipAnalytic;
	}

	public override void Set((int Id, IReadOnlyList<RewardWithManyConditions> Rewards) source)
	{
		base.Set(source);
		_smallCardsViewManager.HideAll();
		_messageDropViewManager.HideAll();
		foreach (RewardWithManyConditions item in base.Source.Rewards)
		{
			LinkedContent content = item.Content;
			if (content is LootboxLinkedContent lootboxLinkedContent)
			{
				_smallCardsViewManager.Display(RewType.Lootbox, item.Selector, (int)lootboxLinkedContent.GetRarity());
				continue;
			}
			if (content is PresentLinkedContent presentLinkedContent)
			{
				_smallCardsViewManager.Display(RewType.Resource, item.Selector, null, presentLinkedContent.Quantity);
				continue;
			}
			if (content is CurrencyLinkedContent currencyLinkedContent)
			{
				_smallCardsViewManager.Display(RewType.Resource, item.Selector, null, currencyLinkedContent.Quantity);
				continue;
			}
			if (content is CardLinkedContent cardLinkedContent)
			{
				_smallCardsViewManager.Display(RewType.Characters, item.Selector, null, cardLinkedContent.Quantity);
				continue;
			}
			if (content is MergeItemLinkedContent mergeItemLinkedContent)
			{
				_smallCardsViewManager.Display(RewType.MergeItem, item.Selector, null, mergeItemLinkedContent.Quantity);
				continue;
			}
			if (content is SkinLinkedContent skinLinkedContent)
			{
				_smallCardsViewManager.Display(RewType.Skin, item.Selector, (int)skinLinkedContent.Skin.Rarity);
				continue;
			}
			if (content is BoosterLinkedContent)
			{
				_smallCardsViewManager.Display(RewType.Booster, item.Selector);
				continue;
			}
			if (content is DecorationLinkedContent)
			{
				_smallCardsViewManager.Display(RewType.Decorations, item.Selector);
				continue;
			}
			throw new Exception($"No behaviour for this content.Type: {content.Type}").LogException();
		}
		CheckMessage();
	}

	protected override void TryClaimReward()
	{
		foreach (RewardWithManyConditions item in base.Source.Rewards)
		{
			item.TryCollectReward();
		}
		_rewardClaimed?.OnNext(_id);
		_relationshipAnalytic.SendRewardReceivedEvent(_id);
	}

	private void CheckMessage()
	{
		foreach (DialogueConfigMapper item in _dialogueConfigMapperManager.Collection)
		{
			UnlockType[] unlockTypes = item.UnlockTypes;
			for (int i = 0; i < unlockTypes.Length; i++)
			{
				if (unlockTypes[i] == UnlockType.RelationshipRewarded)
				{
					int obj = int.Parse(item.UnlockValues[i].Split(':')[1]);
					if (_id.Equals(obj))
					{
						_messageDropViewManager.GetView().transform.SetAsLastSibling();
					}
				}
			}
		}
	}
}
