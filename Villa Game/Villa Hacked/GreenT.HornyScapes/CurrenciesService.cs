using System.Collections.Generic;
using System.Linq;
using GreenT.Types;
using StripClub.Model;
using StripClub.Model.Data;
using StripClub.Model.Shop.Data;
using UnityEngine;

namespace GreenT.HornyScapes;

public sealed class CurrenciesService
{
	private const string MINIEVENT_CURRENCY_LOCALIZATION_KEY = "ui.minievent.currency.{0}";

	private const string EVENTXP_CURRENCY_LOCALIZATION_KEY = "ui.eventXP.currency.{0}";

	private readonly Currencies _currencies;

	private readonly GameSettings _gameSettings;

	private readonly BundlesProviderBase _bundlesProvider;

	private readonly SimpleCurrencyFactory _simpleCurrencyFactory;

	private readonly Dictionary<CurrencyType, string> _localizationKeys;

	public CurrenciesService(Currencies currencies, GameSettings gameSettings, BundlesProviderBase bundlesProvider, SimpleCurrencyFactory simpleCurrencyFactory)
	{
		_currencies = currencies;
		_gameSettings = gameSettings;
		_bundlesProvider = bundlesProvider;
		_simpleCurrencyFactory = simpleCurrencyFactory;
		_gameSettings.CurrencySettings.SetupCurrenciesService(this);
		_gameSettings.CurrencyPlaceholder.SetupCurrenciesService(this);
		_localizationKeys = new Dictionary<CurrencyType, string>
		{
			{
				CurrencyType.MiniEvent,
				"ui.minievent.currency.{0}"
			},
			{
				CurrencyType.EventXP,
				"ui.eventXP.currency.{0}"
			}
		};
	}

	public void CheckoutCurrency(CurrencyType currencyType, CompositeIdentificator currencyIdentificator)
	{
		if (currencyType == CurrencyType.MiniEvent && !_currencies.Contains(currencyType, currencyIdentificator))
		{
			SimpleCurrency simpleCurrency = _simpleCurrencyFactory.Create(currencyType, 0, null, currencyIdentificator[0]);
			_currencies.Set(currencyType, simpleCurrency, currencyIdentificator);
		}
	}

	public void CheckoutCurrencySettings(CurrencyType currencyType, CompositeIdentificator currencyIdentificator)
	{
		if ((currencyType != CurrencyType.MiniEvent && currencyType != CurrencyType.EventXP) || currencyIdentificator.Identificators == null || !currencyIdentificator.Identificators.Any())
		{
			return;
		}
		bool flag = _gameSettings.CurrencySettings.Contains(currencyType, currencyIdentificator);
		bool flag2 = _gameSettings.CurrencyPlaceholder.Contains(currencyType, currencyIdentificator);
		if (!flag || !flag2)
		{
			CurrencySettings currencySettings = new CurrencySettings();
			SetupCurrencySettings(currencySettings, currencyType, currencyIdentificator);
			if (!flag)
			{
				_gameSettings.CurrencySettings.TryAdd(currencyType, currencySettings, currencyIdentificator);
			}
			if (!flag2)
			{
				_gameSettings.CurrencyPlaceholder.TryAdd(currencyType, currencySettings, currencyIdentificator);
			}
		}
		else
		{
			if (flag)
			{
				SetupCurrencySettings(_gameSettings.CurrencySettings.Get(currencyType, currencyIdentificator), currencyType, currencyIdentificator);
			}
			if (flag2)
			{
				SetupCurrencySettings(_gameSettings.CurrencyPlaceholder.Get(currencyType, currencyIdentificator), currencyType, currencyIdentificator);
			}
		}
	}

	private void SetupCurrencySettings(CurrencySettings currencySettings, CurrencyType currencyType, CompositeIdentificator currencyIdentificator)
	{
		Sprite icon = GetIcon(currencyType, currencyIdentificator);
		string localizationKey = GetLocalizationKey(currencyType, currencyIdentificator);
		currencySettings.SetSprite(icon);
		currencySettings.SetAlternativeSprite(icon);
		currencySettings.SetLocalization(localizationKey);
	}

	private Sprite GetIcon(CurrencyType currencyType, CompositeIdentificator currencyIdentificator)
	{
		string bundleName = $"{currencyType}_{currencyIdentificator[0]}";
		return _bundlesProvider.TryFindInConcreteBundle<Sprite>(ContentSource.Currencies, bundleName);
	}

	private string GetLocalizationKey(CurrencyType currencyType, CompositeIdentificator currencyIdentificator)
	{
		return string.Format(_localizationKeys[currencyType], currencyIdentificator[0]);
	}
}
