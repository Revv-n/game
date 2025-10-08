using System;
using GreenT.HornyScapes.Bank.UI;
using GreenT.HornyScapes.Constants;
using GreenT.UI;
using StripClub.Model;
using StripClub.Model.Shop;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Monetization;

public class Purchaser : IDisposable
{
	private readonly SignalBus _signalBus;

	private readonly IConstants<int> _intConstants;

	private readonly IWindowsManager _windowsManager;

	private readonly IMonetizationAdapter _iapPurchaser;

	private readonly IMonetizationRecorder _monetizationRecorder;

	private readonly IRegionPriceResolver _regionPriceResolver;

	private readonly Subject<bool> _onResult = new Subject<bool>();

	private readonly CompositeDisposable _onBuyStream = new CompositeDisposable();

	public IObservable<bool> OnResult => _onResult;

	public Purchaser(IMonetizationAdapter iapPurchaser, IWindowsManager windowsManager, IConstants<int> intConstants, SignalBus signalBus, IMonetizationRecorder monetizationRecorder, IRegionPriceResolver regionPriceResolver)
	{
		_iapPurchaser = iapPurchaser;
		_windowsManager = windowsManager;
		_signalBus = signalBus;
		_monetizationRecorder = monetizationRecorder;
		_regionPriceResolver = regionPriceResolver;
		_intConstants = intConstants;
	}

	public void TryPurchase(ValuableLot<decimal> lot, string paymentID, string itemName, string itemDescription = null, string itemImageUrl = null)
	{
		if (lot.Price.Currency.IsRealCurrency())
		{
			_onBuyStream?.Clear();
			IObservable<Unit> onSuccess = _iapPurchaser.OnSuccess;
			IObservable<string> onFailed = _iapPurchaser.OnFailed;
			onSuccess.TakeUntil(onFailed).Take(1).Do(delegate
			{
				InnerPurchase(lot);
			})
				.SelectMany((Unit _) => _monetizationRecorder.ApproveLast())
				.Subscribe(delegate
				{
				})
				.AddTo(_onBuyStream);
			onFailed.TakeUntil(onSuccess).Take(1).Do(delegate
			{
			})
				.Subscribe(delegate
				{
					_onResult.OnNext(value: false);
				})
				.AddTo(_onBuyStream);
			_iapPurchaser.BuyProduct(paymentID, lot.MonetizationID, lot.ID, lot.Price.Value.ToString(), _regionPriceResolver.CurrentRegion, itemName, itemDescription, itemImageUrl, lot.Price.Currency.ToString());
		}
		else
		{
			InnerPurchase(lot);
		}
	}

	private void InnerPurchase(ValuableLot<decimal> lot)
	{
		if (!lot.Purchase())
		{
			if (lot.Price.Currency != CurrencyType.Real)
			{
				OpenBank(lot.Price.Currency);
			}
			_onResult.OnNext(value: false);
		}
		else
		{
			lot.SendPurchaseNotification();
			_onResult.OnNext(value: true);
		}
	}

	private void OpenBank(CurrencyType currency)
	{
		if (currency != CurrencyType.MiniEvent)
		{
			_windowsManager.Get<BankWindow>().Open();
			switch (currency)
			{
			case CurrencyType.Hard:
				_signalBus.Fire(new OpenTabSignal(_intConstants["banktab_no_hards"]));
				break;
			case CurrencyType.Soft:
				_signalBus.Fire(new OpenTabSignal(_intConstants["banktab_no_coins"]));
				break;
			}
		}
	}

	public void Dispose()
	{
		_onBuyStream.Dispose();
	}
}
