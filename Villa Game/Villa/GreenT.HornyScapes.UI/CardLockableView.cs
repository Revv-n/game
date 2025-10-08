using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Card.UI;
using GreenT.HornyScapes.Characters;
using GreenT.HornyScapes.Dates.Models;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.Relationships.Models;
using GreenT.HornyScapes.Relationships.Providers;
using GreenT.Localizations;
using StripClub.Model;
using StripClub.Model.Cards;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.UI;

public class CardLockableView : ProgressCardView
{
	[Serializable]
	public class UnlockDescriptionDictionary : SerializableDictionary<UnlockType, string>
	{
	}

	private static readonly string eventName = "content.event.{0}.name";

	private static readonly string localizationKey = "ui.collection.character.{0}.unlock";

	[SerializeField]
	private GameObject unlockCardView;

	[SerializeField]
	private GameObject lockPlug;

	[SerializeField]
	private TextMeshProUGUI unlockDescription;

	[SerializeField]
	private Image lockImage;

	[SerializeField]
	private int girlGroupID = 1;

	[SerializeField]
	private CharacterPlaceholderSpriteCollection placeholderCollection;

	[SerializeField]
	private GameObject _notification;

	[SerializeField]
	private Image _dateNotification;

	private LocalizationService _localizationService;

	private RelationshipProvider _relationshipProvider;

	private IDisposable _relationshipStream;

	[Inject]
	public void Init(LocalizationService localizationService, RelationshipProvider relationshipProvider)
	{
		_localizationService = localizationService;
		_relationshipProvider = relationshipProvider;
	}

	public override void Set(ICard card)
	{
		base.Set(card);
		bool flag = base.promote != null;
		lockPlug.SetActive(!flag);
		unlockCardView.SetActive(flag);
		if (!flag)
		{
			SetupLockView(card);
		}
		_relationshipStream?.Dispose();
		if (_notification != null && TryGetRelationship(out var relationship))
		{
			_relationshipStream = relationship.WasComingSoonDates.Subscribe(_notification.SetActive);
			bool active = relationship.Rewards.Any((IReadOnlyList<RewardWithManyConditions> rewards) => rewards.Any((RewardWithManyConditions reward) => reward.Content is DateLinkedContent));
			if (_dateNotification != null)
			{
				_dateNotification.gameObject.SetActive(active);
			}
		}
		else
		{
			_dateNotification.gameObject.SetActive(value: false);
		}
	}

	private void SetupLockView(ICard card)
	{
		unlockDescription.text = _localizationService.Text(string.Format(_localizationService.Text(localizationKey), card.ID));
		SetLockView(card);
	}

	private void SetLockView(ICard card)
	{
		lockImage.sprite = placeholderCollection.Get(card.ID);
	}

	private bool TryGetRelationship(out Relationship relationship)
	{
		int relationsipId = (base.Source as GreenT.HornyScapes.Characters.CharacterInfo).RelationsipId;
		relationship = _relationshipProvider.Get(relationsipId);
		return relationship != null;
	}
}
