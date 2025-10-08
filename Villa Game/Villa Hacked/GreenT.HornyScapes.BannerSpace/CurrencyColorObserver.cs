using System;
using StripClub.Model.Shop;
using StripClub.UI.Shop;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.BannerSpace;

[RequireComponent(typeof(PriceWithFreeView))]
public class CurrencyColorObserver : MonoBehaviour
{
	[SerializeField]
	private PriceWithFreeView _button;

	private ICurrencyProcessor _currencyProcessor;

	private IDisposable _colorStream;

	[Inject]
	public void Initialization(ICurrencyProcessor currencyProcessor)
	{
		_currencyProcessor = currencyProcessor;
	}

	public void Set(Price<int> price)
	{
		_button.Set(price);
		int valueColor = IsCurrencyEnough(price);
		_colorStream?.Dispose();
		_button.SetValueColor(valueColor);
		_colorStream = ObservableExtensions.Subscribe<int>(Observable.Select<int, int>((IObservable<int>)_currencyProcessor.GetCountReactiveProperty(price.Currency), (Func<int, int>)((int _) => IsCurrencyEnough(price))), (Action<int>)_button.SetValueColor);
	}

	private int IsCurrencyEnough(Price<int> price)
	{
		if (!_currencyProcessor.IsEnough(price.Currency, price.Value))
		{
			return 1;
		}
		return 0;
	}

	private void OnValidate()
	{
		if ((object)_button == null)
		{
			_button = GetComponent<PriceWithFreeView>();
		}
	}
}
