using System;
using GreenT.HornyScapes.Characters;
using GreenT.HornyScapes.Characters.Skins;
using GreenT.HornyScapes.Relationships.Views;
using GreenT.Localizations;
using StripClub.Model.Cards;
using StripClub.UI;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.UI;

public class GeneralCardView : CardView
{
	[SerializeField]
	protected CardPictureSelector pictureSelector;

	[SerializeField]
	protected TextMeshProUGUI objectName;

	[SerializeField]
	protected StatableComponent statableComponent;

	[SerializeField]
	protected RelationshipStatusView relationshipStatusView;

	private SkinManager skinManager;

	private CharacterSettingsManager characterManager;

	private IDisposable disposable;

	private LocalizationService _localizationService;

	[Inject]
	public void Init(SkinManager skinManager, CharacterSettingsManager characterManager, LocalizationService localizationService)
	{
		this.skinManager = skinManager;
		this.characterManager = characterManager;
		_localizationService = localizationService;
	}

	public override void Set(ICard card)
	{
		base.Set(card);
		statableComponent.Set((int)card.Rarity);
		pictureSelector.Init(card);
		relationshipStatusView.Set(card);
		CheckStatusActive();
		TrackNameLocalization();
	}

	protected virtual void OnEnable()
	{
		if (base.Source != null)
		{
			TrackNameLocalization();
		}
	}

	protected void TrackNameLocalization()
	{
		disposable?.Dispose();
		if (objectName == null)
		{
			return;
		}
		IObservable<string> observable = null;
		if (base.Source is GreenT.HornyScapes.Characters.CharacterInfo characterInfo)
		{
			CharacterSettings characterSettings = characterManager.Get(characterInfo.ID);
			string key = EvaluateNameKey(characterSettings);
			observable = _localizationService.ObservableText(key);
			if (characterSettings != null)
			{
				IObservable<string> observable2 = characterSettings.OnUpdate.Select(EvaluateNameKey).SelectMany((Func<string, IObservable<string>>)_localizationService.ObservableText);
				observable = observable.Merge(observable2);
			}
		}
		else
		{
			observable = _localizationService.ObservableText(base.Source.NameKey);
		}
		disposable = observable.Subscribe(delegate(string _value)
		{
			objectName.text = _value;
		}).AddTo(this);
	}

	private string EvaluateNameKey(CharacterSettings settings = null)
	{
		string nameKey = base.Source.NameKey;
		if (settings != null && settings.SkinID != 0)
		{
			nameKey = skinManager.Get(settings.SkinID).NameKey;
		}
		return nameKey;
	}

	private void CheckStatusActive()
	{
		bool active = base.Source is ICharacter character && character.RelationsipId != 0;
		relationshipStatusView.gameObject.SetActive(active);
	}

	protected virtual void OnDisable()
	{
		disposable?.Dispose();
	}
}
