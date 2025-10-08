using System;
using GreenT.HornyScapes._HornyScapes._Scripts.Cheats;
using GreenT.HornyScapes.Analytics;
using StripClub.Model;
using StripClub.Model.Shop;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Cheats;

public class CheatPierCurrency : MonoBehaviour, IDisposable
{
	private ICurrencyProcessor currencyProcessor;

	public Button AddPierCurrency;

	public TMP_InputField Amount;

	private IDisposable _addCurrencyDisposable;

	private const CurrencyAmplitudeAnalytic.SourceType SOURCE_TYPE = CurrencyAmplitudeAnalytic.SourceType.None;

	[Inject]
	private void InnerInit(ICurrencyProcessor currencyProcessor)
	{
		this.currencyProcessor = currencyProcessor;
		Dispose();
		_addCurrencyDisposable = ObservableExtensions.Subscribe<Unit>(UnityUIComponentExtensions.OnClickAsObservable(AddPierCurrency), (Action<Unit>)delegate
		{
			OnAddPierCurrency();
		});
	}

	private void OnAddPierCurrency()
	{
		currencyProcessor.TryChangeAmount(CurrencyType.Jewel, ReadAmountValue());
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
	}
}
