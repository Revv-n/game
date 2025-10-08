using System;
using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.Constants;
using GreenT.HornyScapes.Tasks.UI;
using GreenT.Types;
using GreenT.UI;
using StripClub.Model.Data;
using StripClub.Model.Shop;
using StripClub.UI.Shop;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes;

public abstract class BaseRestoreEnergyView<TEnergyRestore, TRestoreEnergyPopup> : EventEnergyBaseView where TEnergyRestore : BaseEnergyRestore where TRestoreEnergyPopup : BaseRestoreEnergyPopup
{
	[SerializeField]
	private ImageSpriteStates statableBackground;

	[Header("Buy option:")]
	[SerializeField]
	private PriceWithFreeView statableButtonView;

	private TEnergyRestore _mainEnergyRestore;

	private CurrencyProcessor _currencyProcessor;

	private CurrencyAmplitudeAnalytic _currencyAmplitudeAnalytic;

	private IWindowsManager _windowsManager;

	protected abstract CurrencyAmplitudeAnalytic.SourceType AmplitudeSourceType { get; }

	[Inject]
	private void InnerInit(CurrencyProcessor currencyProcessor, CurrencyAmplitudeAnalytic currencyAmplitudeAnalytic, IWindowsManager windowsManager, IConstants<int> intConstants)
	{
		_currencyProcessor = currencyProcessor;
		_windowsManager = windowsManager;
		_currencyAmplitudeAnalytic = currencyAmplitudeAnalytic;
	}

	private void OnEnable()
	{
		SetButtonColor();
	}

	public void SetBackground(bool isSingle)
	{
		statableBackground.Set((!isSingle) ? 1 : 0);
	}

	public void Set(TEnergyRestore mainEnergyRestore)
	{
		SetStartValue(mainEnergyRestore);
		_mainEnergyRestore = mainEnergyRestore;
		_mainEnergyRestore.PriceLogics.OnPriceUpdate.Do(delegate(Price<int> price)
		{
			statableButtonView.Set(price);
		}).SelectMany((Func<Price<int>, IObservable<int>>)IsEnoughCurrencyStream).Subscribe(statableButtonView.SetValueColor)
			.AddTo(this);
		buyButton.OnClickAsObservable().Subscribe(delegate
		{
			TryPurchase();
		}).AddTo(this);
		SetButtonColor();
	}

	private void SetButtonColor()
	{
		if (_mainEnergyRestore != null)
		{
			statableButtonView.SetValueColor(GetButtonColorValue(_mainEnergyRestore.PriceLogics.Price));
		}
	}

	private void SetStartValue(TEnergyRestore mainEnergyRestore)
	{
		Price<int> price = mainEnergyRestore.PriceLogics.Price;
		statableButtonView.Set(price);
		statableButtonView.SetValueColor(GetButtonColorValue(price));
	}

	private IObservable<int> IsEnoughCurrencyStream(Price<int> price)
	{
		return from _ in _currencyProcessor.GetCountReactiveProperty(price.Currency, price.CompositeIdentificator)
			select GetButtonColorValue(price);
	}

	private int GetButtonColorValue(Price<int> price)
	{
		Cost cost = new Cost(price.Value, price.Currency);
		if (!_currencyProcessor.IsEnough(cost, price.CompositeIdentificator))
		{
			return 1;
		}
		return 0;
	}

	protected override void TryPurchase()
	{
		bool isFree = _mainEnergyRestore.PriceLogics.IsFree;
		if (_mainEnergyRestore.Purchase())
		{
			_windowsManager.Get<TRestoreEnergyPopup>().Close();
			if (!isFree)
			{
				Price<int> price = _mainEnergyRestore.PriceLogics.Price;
				_currencyAmplitudeAnalytic.SendSpentEvent(price.Currency, price.Value, AmplitudeSourceType, ContentType.Main, price.CompositeIdentificator);
			}
		}
		else
		{
			OpenBank();
		}
	}

	private void OnDestroy()
	{
		_mainEnergyRestore?.Dispose();
	}
}
