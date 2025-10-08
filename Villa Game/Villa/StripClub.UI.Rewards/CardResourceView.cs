using System;
using System.Numerics;
using GreenT.HornyScapes;
using GreenT.Localizations;
using GreenT.Types;
using StripClub.Model;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace StripClub.UI.Rewards;

public class CardResourceView : ResourceView
{
	[Serializable]
	public struct Settings
	{
		public Color TextColor;

		public Sprite Icon;

		public Sprite IconBackplate;

		public string localizationKeySubName;
	}

	[Serializable]
	public class SettingsDictionary : SerializableDictionary<CurrencyType, Settings>
	{
	}

	[SerializeField]
	private TextMeshProUGUI quantity;

	[SerializeField]
	private TextMeshProUGUI resourceName;

	[SerializeField]
	private TextMeshProUGUI subName;

	[SerializeField]
	private Image image;

	[SerializeField]
	private Image backplate;

	[SerializeField]
	private Image cardBack;

	[SerializeField]
	private SettingsDictionary settingsDictionary;

	private GreenT.HornyScapes.GameSettings _gameSettings;

	private LocalizationService _localizationService;

	private IDisposable _iconChangeStream;

	private CompositeDisposable _localizationDisposables = new CompositeDisposable();

	[Inject]
	public void Construct(GreenT.HornyScapes.GameSettings gameSettings, LocalizationService localizationService)
	{
		_gameSettings = gameSettings;
		_localizationService = localizationService;
	}

	public override void Set(CurrencyType currency, BigInteger count, CompositeIdentificator compositeIdentificator)
	{
		base.Set(currency, count, compositeIdentificator);
		Settings currentSettings = settingsDictionary[currency];
		CardBackView(showState: false);
		_localizationDisposables.Clear();
		string text2 = _gameSettings.CurrencySettings[currency, compositeIdentificator].Key;
		if (currency == CurrencyType.MiniEvent)
		{
			text2 = string.Format(text2, compositeIdentificator[0]);
		}
		_localizationService.ObservableText(text2).Subscribe(delegate(string text)
		{
			resourceName.text = text;
		}).AddTo(_localizationDisposables);
		resourceName.color = currentSettings.TextColor;
		string localizationKeySubName = currentSettings.localizationKeySubName;
		(from translated in _localizationService.ObservableText(localizationKeySubName)
			select (currency != CurrencyType.MiniEvent) ? translated : string.Format(translated, compositeIdentificator[0])).Subscribe(delegate(string text)
		{
			subName.text = text;
		}).AddTo(_localizationDisposables);
		backplate.sprite = currentSettings.IconBackplate;
		TextMeshProUGUI textMeshProUGUI = quantity;
		BigInteger bigInteger = count;
		textMeshProUGUI.text = "+" + bigInteger;
		ApplyImage(currency, currentSettings, compositeIdentificator);
	}

	private void ApplyImage(CurrencyType currency, Settings currentSettings, CompositeIdentificator compositeIdentificator)
	{
		_iconChangeStream?.Dispose();
		if (!_gameSettings.CurrencySettings.TryGetValue(currency, out var currencySettings, compositeIdentificator))
		{
			Debug.LogException(new ArgumentOutOfRangeException($"Dictionary [{_gameSettings.CurrencySettings.GetType()} missing Type {currency}]"));
			return;
		}
		if (!IgnoreDefault(currency))
		{
			image.sprite = currentSettings.Icon;
			return;
		}
		Sprite placeholder;
		switch (currency)
		{
		case CurrencyType.BP:
			placeholder = _gameSettings.CurrencyPlaceholder[currency, compositeIdentificator].AlternativeSprite;
			break;
		case CurrencyType.MiniEvent:
			placeholder = _gameSettings.CurrencySettings[currency, compositeIdentificator].AlternativeSprite;
			break;
		default:
			placeholder = currentSettings.Icon;
			break;
		}
		ApplyData(currencySettings.AlternativeSprite, placeholder);
		_iconChangeStream = currencySettings.ObserveEveryValueChanged((CurrencySettings actualSettings) => actualSettings.AlternativeSprite).Subscribe(delegate(Sprite currentValue)
		{
			ApplyData(currentValue, placeholder);
		});
	}

	private bool IgnoreDefault(CurrencyType currency)
	{
		if (currency != CurrencyType.BP && currency != CurrencyType.Event && currency != CurrencyType.EventXP)
		{
			return currency == CurrencyType.MiniEvent;
		}
		return true;
	}

	private void ApplyData(Sprite sprite, Sprite placeholder)
	{
		image.sprite = ((sprite == null) ? placeholder : sprite);
	}

	public void SetActiveQuantity(bool state)
	{
		quantity.gameObject.SetActive(state);
	}

	public void CardBackView(bool showState)
	{
		cardBack.gameObject.SetActive(showState);
	}

	private void OnDestroy()
	{
		_iconChangeStream?.Dispose();
		_localizationDisposables.Dispose();
	}
}
