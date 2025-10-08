using System.Collections.Generic;
using GreenT.HornyScapes.Characters;
using StripClub.Model.Cards;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.UI;

public class CardPictureSelector : MonoBehaviour
{
	[SerializeField]
	private IntImageDictionary imageDictionary;

	[SerializeField]
	private int defaultGroupID = 1;

	private CharacterSettingsManager characterSettingsManager;

	public ReactiveProperty<CardPictures> Current { get; private set; } = new ReactiveProperty<CardPictures>();


	public ICard Card { get; private set; }

	[Inject]
	private void Init(CharacterSettingsManager characterSettingsManager)
	{
		this.characterSettingsManager = characterSettingsManager;
	}

	public void Init(ICard card)
	{
		Card = card;
		if (!imageDictionary.TryGetValue(card.GroupID, out var value))
		{
			value = imageDictionary[defaultGroupID];
		}
		if (Current.Value != value)
		{
			foreach (KeyValuePair<int, CardPictures> item in imageDictionary)
			{
				item.Value.gameObject.SetActive(item.Value == value);
			}
		}
		Current.Value = value;
		if (card is ICharacter character)
		{
			CharacterSettings characterSettings = characterSettingsManager.Get(character.ID);
			if (characterSettings != null)
			{
				Current.Value.Init(characterSettings);
			}
		}
	}
}
