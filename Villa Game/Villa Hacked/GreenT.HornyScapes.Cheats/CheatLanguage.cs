using System;
using System.Collections.Generic;
using GreenT.Localizations;
using GreenT.Localizations.Data;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Cheats;

public class CheatLanguage : MonoBehaviour
{
	[SerializeField]
	private Button _buttonChangeLanguage;

	[SerializeField]
	private TextMeshProUGUI _textChangeLanguage;

	private LocalizationVariantsProvider _variantsProvider;

	private LocalizationState _localizationState;

	private CompositeDisposable _compositeDisposable;

	private List<string> _languages = new List<string>();

	private UIClickSuppressor _mouseSuppressor;

	[Inject]
	public void Init(LocalizationState localizationState, LocalizationVariantsProvider variantsProvider, UIClickSuppressor uIClickSuppressor)
	{
		_localizationState = localizationState;
		_variantsProvider = variantsProvider;
		_mouseSuppressor = uIClickSuppressor;
	}

	private void Awake()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		_compositeDisposable = new CompositeDisposable();
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<LocalizationVariant>(_variantsProvider.OnNew, (Action<LocalizationVariant>)AddLanguage), (ICollection<IDisposable>)_compositeDisposable);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<string>(_localizationState.OnLanguageChange, (Action<string>)OnLanguageChange), (ICollection<IDisposable>)_compositeDisposable);
		_buttonChangeLanguage.onClick.AddListener(NextLanguge);
	}

	private void OnLanguageChange(string language)
	{
		_textChangeLanguage.text = _localizationState.CurrentLanguage;
	}

	private void AddLanguage(LocalizationVariant variant)
	{
		_languages.Add(variant.Key);
	}

	private void NextLanguge()
	{
		if (_languages.Count == 0)
		{
			foreach (LocalizationVariant item in _variantsProvider.Collection)
			{
				_languages.Add(item.Key);
			}
		}
		_mouseSuppressor.SuppressClick();
		int num = _languages.IndexOf(_localizationState.CurrentLanguage);
		if (num == -1)
		{
			num = 0;
		}
		int index = (num + 1) % _languages.Count;
		string text = _languages[index];
		_localizationState.UpdateLanguage(text);
		_localizationState.UpdateLocalLanguage(text);
		_textChangeLanguage.text = text;
	}

	private void OnDestroy()
	{
		_compositeDisposable.Dispose();
		_buttonChangeLanguage.onClick.RemoveAllListeners();
	}
}
