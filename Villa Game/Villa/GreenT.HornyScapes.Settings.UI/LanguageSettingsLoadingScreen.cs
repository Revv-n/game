using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Animations;
using GreenT.Localizations;
using GreenT.Localizations.Data;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Settings.UI;

public class LanguageSettingsLoadingScreen : MonoBehaviour
{
	[Serializable]
	public struct CountToColumns
	{
		public int Count;

		public int Columns;
	}

	[SerializeField]
	private LanguageSettingsButton _prefabButton;

	[SerializeField]
	private RectTransform _buttonRoot;

	[SerializeField]
	private GridLayoutGroup _gridLayoutGroup;

	[SerializeField]
	private CountToColumns[] _countToColumns;

	[SerializeField]
	private PopupWindow _popupWindow;

	private LocalizationVariantsProvider _variantsProvider;

	private LocalizationState _localizationState;

	private readonly Dictionary<string, LanguageSettingsButton> _buttons = new Dictionary<string, LanguageSettingsButton>();

	private CompositeDisposable _compositeDisposable;

	public Canvas Canvas { get; private set; }

	[Inject]
	public void Init(LocalizationState localizationState, LocalizationVariantsProvider variantsProvider)
	{
		_localizationState = localizationState;
		_variantsProvider = variantsProvider;
	}

	private void Awake()
	{
		_compositeDisposable = new CompositeDisposable();
		_variantsProvider.OnNew.Subscribe(AddVariant).AddTo(_compositeDisposable);
		_localizationState.OnLanguageChange.Subscribe(OnLanguageChange).AddTo(_compositeDisposable);
		foreach (LocalizationVariant item in _variantsProvider.Collection)
		{
			AddVariant(item);
		}
	}

	private void OnLanguageChange(string language)
	{
		foreach (LanguageSettingsButton value in _buttons.Values)
		{
			if (value.Key.Equals(language))
			{
				value.Select();
			}
			else
			{
				value.Deselect();
			}
		}
	}

	private void AddVariant(LocalizationVariant variant)
	{
		LanguageSettingsButton languageSettingsButton = UnityEngine.Object.Instantiate(_prefabButton, _buttonRoot);
		languageSettingsButton.Initialization(variant.Name, variant.Key);
		if (variant.Key.Equals(_localizationState.CurrentLanguage))
		{
			languageSettingsButton.Select();
		}
		else
		{
			languageSettingsButton.Deselect();
		}
		_buttons.Add(variant.Key, languageSettingsButton);
		UpdateGridUI();
		languageSettingsButton.OnClick.Where((LanguageSettingsButton x) => !x.Key.Equals(_localizationState.CurrentLanguage)).Subscribe(ChangeLocalization);
	}

	private void UpdateGridUI()
	{
		RectTransform component = _gridLayoutGroup.GetComponent<RectTransform>();
		int num = -1;
		CountToColumns[] countToColumns = _countToColumns;
		for (int i = 0; i < countToColumns.Length; i++)
		{
			CountToColumns countToColumns2 = countToColumns[i];
			if (_buttons.Count <= countToColumns2.Count)
			{
				num = countToColumns2.Columns;
				break;
			}
		}
		if (num > 0 && _gridLayoutGroup.constraintCount != num)
		{
			_gridLayoutGroup.constraintCount = num;
			LayoutRebuilder.ForceRebuildLayoutImmediate(component);
		}
	}

	private void ChangeLocalization(LanguageSettingsButton selectButton)
	{
		_localizationState.UpdateLanguage(selectButton.Key);
	}

	private void OnDestroy()
	{
		_compositeDisposable.Clear();
	}

	public void SetCanvas(Canvas canvas)
	{
		Canvas = canvas;
		_popupWindow.Canvas = Canvas;
	}
}
