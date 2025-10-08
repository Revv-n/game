using System;
using System.Linq;
using GreenT.Localizations;
using GreenT.Localizations.Data;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Settings.UI;

public class CurrentLanguageButton : MonoBehaviour
{
	[SerializeField]
	private TMP_Text _text;

	private LocalizationState _localizationState;

	private LocalizationVariantsProvider _variantsProvider;

	[Inject]
	public void Init(LocalizationState localizationState, LocalizationVariantsProvider variantsProvider)
	{
		_localizationState = localizationState;
		_variantsProvider = variantsProvider;
	}

	private void Awake()
	{
		ObservableExtensions.Subscribe<string>(_localizationState.OnLanguageChange, (Action<string>)OnLanguageChange);
		ObservableExtensions.Subscribe<LocalizationVariant>(_variantsProvider.OnNew, (Action<LocalizationVariant>)AddVariant);
	}

	private void Start()
	{
		OnLanguageChange(_localizationState.CurrentLanguage);
	}

	private void OnLanguageChange(string language)
	{
		LocalizationVariant localizationVariant = _variantsProvider.Collection.FirstOrDefault((LocalizationVariant p) => p.Key.Equals(language));
		_text.text = ((localizationVariant == null) ? "" : localizationVariant.Name);
	}

	private void AddVariant(LocalizationVariant variant)
	{
		if (string.IsNullOrEmpty(_text.text) && variant.Key.Equals(_localizationState.CurrentLanguage))
		{
			OnLanguageChange(_localizationState.CurrentLanguage);
		}
	}
}
