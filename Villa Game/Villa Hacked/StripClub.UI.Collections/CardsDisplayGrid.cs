using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Characters;
using GreenT.HornyScapes.Dates.Extensions;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.Relationships.Providers;
using GreenT.HornyScapes.UI;
using GreenT.Localizations;
using GreenT.Types;
using Merge.Meta.RoomObjects;
using StripClub.Model.Cards;
using TMPro;
using UnityEngine;
using Zenject;

namespace StripClub.UI.Collections;

public class CardsDisplayGrid : MonoBehaviour
{
	private const string nameKey = "content.collections.tab.{0}.name";

	private const string progressFormat = "ui.collections.tab.progress";

	[SerializeField]
	private TextMeshProUGUI groupName;

	[SerializeField]
	private TextMeshProUGUI unlockProgress;

	private CardsCollection cards;

	private CollectionCardView.Manager cardViewManager;

	private LocalizationService _localizationService;

	private RelationshipProvider _relationshipProvider;

	private int lastUnlockedSibling;

	private int currentGroup = 1;

	[Inject]
	public void Init(CardsCollection cards, CollectionCardView.Manager cardViewManager, LocalizationService localizationService, RelationshipProvider relationshipProvider)
	{
		this.cardViewManager = cardViewManager;
		this.cards = cards;
		_localizationService = localizationService;
		_relationshipProvider = relationshipProvider;
	}

	private void OnEnable()
	{
		Display(currentGroup);
	}

	public void Display(int groupID)
	{
		currentGroup = groupID;
		SetTabName(groupID);
		SetUnlockedCountView(groupID);
		IEnumerable<ICard> cardsToDisplay = cards.Collection.Where((ICard _card) => _card.GroupID.Equals(groupID) && _card.ContentType == ContentType.Main);
		cardsToDisplay = Sort(cardsToDisplay);
		Display(cardsToDisplay);
	}

	private void SetTabName(int groupID)
	{
		string key = $"content.collections.tab.{groupID}.name";
		groupName.text = _localizationService.Text(key);
	}

	private void SetUnlockedCountView(int groupID)
	{
		int num = cards.UnlockedCount(groupID);
		int num2 = cards.Count(groupID);
		unlockProgress.text = string.Format(_localizationService.Text("ui.collections.tab.progress"), num, num2);
	}

	private void Display(IEnumerable<ICard> cardsToDisplay)
	{
		lastUnlockedSibling = 0;
		cardViewManager.HideAll();
		foreach (ICard item in cardsToDisplay)
		{
			Display(item);
		}
	}

	private void Display(ICard card)
	{
		IPromote promoteOrDefault = cards.GetPromoteOrDefault(card);
		CollectionCardView view = cardViewManager.GetView();
		view.Set(card);
		if (promoteOrDefault != null)
		{
			view.NoveltyIndicator.Init(promoteOrDefault);
			view.transform.SetSiblingIndex(lastUnlockedSibling);
			lastUnlockedSibling++;
		}
	}

	private IEnumerable<ICard> Sort(IEnumerable<ICard> cardsToDisplay)
	{
		return (from card in cardsToDisplay.ToList()
			orderby card.Rarity descending, cards.GetPromoteOrDefault(card)?.Level.Value descending, GetStatusRewardIndex(card) descending
			select card).ToList();
	}

	private int GetStatusRewardIndex(ICard card)
	{
		int relationsipId = (card as GreenT.HornyScapes.Characters.CharacterInfo).RelationsipId;
		if (relationsipId == 0 || !_relationshipProvider.TryGet(relationsipId, out var relationship))
		{
			return 0;
		}
		IReadOnlyList<IReadOnlyList<RewardWithManyConditions>> rewards = relationship.Rewards;
		IReadOnlyList<RewardWithManyConditions> readOnlyList = rewards.FirstOrDefault((IReadOnlyList<RewardWithManyConditions> x) => x.All((RewardWithManyConditions x) => x.State.Value == EntityStatus.InProgress || x.State.Value == EntityStatus.Blocked));
		if (readOnlyList != null)
		{
			int num = rewards.IndexOf(readOnlyList) - 1;
			if (num < 0)
			{
				num = 0;
			}
			return num;
		}
		return rewards.Count - 1;
	}
}
