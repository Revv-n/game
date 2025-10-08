using System;
using GreenT.HornyScapes.Analytics;
using GreenT.Types;
using StripClub.Model;
using StripClub.Model.Shop;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Cheats;

public class CheatMiniEvents : MonoBehaviour, IDisposable
{
	private ICurrencyProcessor currencyProcessor;

	public Button AddMiniEventCurrency;

	public Button SpendMiniEventCurrency;

	public TMP_InputField Amount;

	public TMP_InputField FirstIDInputField;

	private IDisposable _addCurrencyDisposable;

	private IDisposable _spendCurrencyDisposable;

	private const CurrencyAmplitudeAnalytic.SourceType SOURCE_TYPE = CurrencyAmplitudeAnalytic.SourceType.None;

	[Inject]
	private void InnerInit(ICurrencyProcessor currencyProcessor)
	{
		this.currencyProcessor = currencyProcessor;
		Dispose();
		_addCurrencyDisposable = AddMiniEventCurrency.OnClickAsObservable().Subscribe(delegate
		{
			OnAddMiniEventCurrency();
		});
		_spendCurrencyDisposable = SpendMiniEventCurrency.OnClickAsObservable().Subscribe(delegate
		{
			OnSpendMiniEventCurrency();
		});
	}

	private void OnAddMiniEventCurrency()
	{
		currencyProcessor.TryAdd(CurrencyType.MiniEvent, ReadAmountValue(), CurrencyAmplitudeAnalytic.SourceType.None, new CompositeIdentificator(ReadFirstIDInputField()));
	}

	private void OnSpendMiniEventCurrency()
	{
		currencyProcessor.TrySpent(CurrencyType.MiniEvent, ReadAmountValue(), new CompositeIdentificator(ReadFirstIDInputField()));
	}

	private int ReadAmountValue()
	{
		int result;
		int result2 = ((!int.TryParse(Amount.text, out result)) ? 1 : result);
		Amount.text = string.Empty;
		return result2;
	}

	private int ReadFirstIDInputField()
	{
		int result;
		int result2 = ((!int.TryParse(FirstIDInputField.text, out result)) ? 1 : result);
		FirstIDInputField.text = string.Empty;
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
