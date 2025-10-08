using System;
using GreenT.HornyScapes.Analytics;
using StripClub.Model;
using StripClub.Model.Shop;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Cheats;

public class CheatEventEnergy : MonoBehaviour, IDisposable
{
	private ICurrencyProcessor currencyProcessor;

	public Button AddEventEnergyCurrency;

	public Button SpendEventEventCurrency;

	public TMP_InputField Amount;

	private IDisposable _addCurrencyDisposable;

	private IDisposable _spendCurrencyDisposable;

	private const CurrencyAmplitudeAnalytic.SourceType SOURCE_TYPE = CurrencyAmplitudeAnalytic.SourceType.None;

	[Inject]
	private void InnerInit(ICurrencyProcessor currencyProcessor)
	{
		this.currencyProcessor = currencyProcessor;
		Dispose();
		_addCurrencyDisposable = AddEventEnergyCurrency.OnClickAsObservable().Subscribe(delegate
		{
			OnAddEventEventCurrency();
		});
		_spendCurrencyDisposable = SpendEventEventCurrency.OnClickAsObservable().Subscribe(delegate
		{
			OnSpendEventEventCurrency();
		});
	}

	private void OnAddEventEventCurrency()
	{
		currencyProcessor.TryAdd(CurrencyType.EventEnergy, ReadAmountValue());
	}

	private void OnSpendEventEventCurrency()
	{
		currencyProcessor.TrySpent(CurrencyType.EventEnergy, ReadAmountValue());
	}

	private int ReadAmountValue()
	{
		int result;
		int result2 = ((!int.TryParse(Amount.text, out result)) ? 1 : result);
		Amount.text = string.Empty;
		return result2;
	}

	private void OnDestroy()
	{
		Dispose();
	}

	public void Dispose()
	{
		_addCurrencyDisposable?.Dispose();
		_spendCurrencyDisposable?.Dispose();
	}
}
