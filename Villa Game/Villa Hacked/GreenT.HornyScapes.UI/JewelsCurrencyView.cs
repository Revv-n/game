using System;
using System.Linq;
using GreenT.Model.Reactive;
using StripClub.Model;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.UI;

public sealed class JewelsCurrencyView : CurrencyView
{
	private const int ZERO = 0;

	private IDisposable _currentAmountStream;

	private ReactiveCollection<CurrencyType> _visibleCurrenciesManager;

	public bool IsAboveZero => current.Value > 0;

	[Inject]
	private void Init(ReactiveCollection<CurrencyType> visibleCurrenciesManager)
	{
		_visibleCurrenciesManager = visibleCurrenciesManager;
	}

	private void OnDestroy()
	{
		_currentAmountStream?.Dispose();
	}

	public override void Display(bool display)
	{
		TryGetReactiveCount();
		TryTrackAmount();
		if (display)
		{
			if (IsAboveZero)
			{
				base.Display(display);
			}
			else
			{
				base.gameObject.SetActive(value: false);
			}
		}
		else
		{
			base.Display(display);
		}
	}

	private void TryTrackAmount()
	{
		if (_currentAmountStream == null)
		{
			_currentAmountStream = ObservableExtensions.Subscribe<int>(Observable.Skip<int>((IObservable<int>)current, 1), (Action<int>)delegate
			{
				UpdateCurrentVisibleCurrencies();
			});
		}
	}

	private void UpdateCurrentVisibleCurrencies()
	{
		CurrencyType[] items = _visibleCurrenciesManager.ToArray();
		_visibleCurrenciesManager.SetItems(items);
	}
}
