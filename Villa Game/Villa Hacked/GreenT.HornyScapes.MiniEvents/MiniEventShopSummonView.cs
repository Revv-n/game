using System;
using System.Collections.Generic;
using GreenT.Localizations;
using StripClub.Model.Data;
using StripClub.Model.Shop;
using StripClub.UI;
using StripClub.UI.Shop;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.MiniEvents;

public class MiniEventShopSummonView : LotView
{
	[SerializeField]
	private TextMeshProUGUI[] _titleTextFields;

	[SerializeField]
	private TextMeshProUGUI[] _descriptionTextFields;

	[Space]
	[SerializeField]
	private PriceWithFreeView _singlePrice;

	[SerializeField]
	private Button _singlePurchaseButton;

	[SerializeField]
	private PriceWithFreeView _wholesalePrice;

	[SerializeField]
	private Button _wholesalePurchaseButton;

	[Space]
	[SerializeField]
	private MiniEventSummonLootboxContentView _lootboxContentView;

	private CompositeDisposable _disposables = new CompositeDisposable();

	private ICurrencyProcessor _currencyProcessor;

	private LocalizationService _localizationService;

	public ReactiveProperty<SummonLot> OnSet = new ReactiveProperty<SummonLot>();

	[Inject]
	public void Init(ICurrencyProcessor currencyProcessor, LocalizationService localizationService)
	{
		_currencyProcessor = currencyProcessor;
		_localizationService = localizationService;
	}

	private void OnEnable()
	{
		if (base.Source != null)
		{
			Set((SummonLot)base.Source);
		}
	}

	private void OnDisable()
	{
		_disposables.Clear();
	}

	private void OnDestroy()
	{
		_disposables.Dispose();
	}

	public override void Set(Lot lot)
	{
		_disposables.Clear();
		base.Set(lot);
		SummonLot summonLot = (SummonLot)lot;
		if (summonLot.AvailableCount > 0 && summonLot.AvailableCount <= summonLot.Received)
		{
			Display(display: false);
			return;
		}
		TextMeshProUGUI[] titleTextFields = _titleTextFields;
		for (int i = 0; i < titleTextFields.Length; i++)
		{
			titleTextFields[i].text = _localizationService.Text(summonLot.TitleKey);
		}
		titleTextFields = _descriptionTextFields;
		for (int i = 0; i < titleTextFields.Length; i++)
		{
			titleTextFields[i].text = _localizationService.Text(summonLot.DescriptionKey);
		}
		SetButtons(summonLot);
		DisplayContent(summonLot);
		OnSet.Value = summonLot;
	}

	private void OnTimeIsUp(GenericTimer obj)
	{
		if (base.Source != null || base.Source is SummonLot)
		{
			SetButtons((SummonLot)base.Source);
		}
	}

	private void DisplayContent(SummonLot summonLot)
	{
		_lootboxContentView.Set(summonLot);
	}

	private void SetButtons(SummonLot summonLot)
	{
		Price<int> price = (summonLot.IsFree ? Price<int>.Free : summonLot.SingleRewardSettings.Price);
		SetupButton(_singlePrice, price, _singlePurchaseButton, wholesale: false);
		Price<int> price2 = summonLot.WholesaleRewardSettings.Price;
		SetupButton(_wholesalePrice, price2, _wholesalePurchaseButton, wholesale: true);
	}

	private void SetupButton(PriceWithFreeView view, Price<int> price, Button purchaseButton, bool wholesale)
	{
		view.Set(price);
		Cost cost = new Cost(price.Value, price.Currency);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<int>(Observable.Select<int, int>((IObservable<int>)_currencyProcessor.GetCountReactiveProperty(price.Currency, price.CompositeIdentificator), (Func<int, int>)((int _) => CurrencyProcessor.IsEnough(cost, price.CompositeIdentificator) ? 1 : 0)), (Action<int>)view.SetValueColor), (ICollection<IDisposable>)_disposables);
		IConnectableObservable<bool> obj = Observable.Publish<bool>(Observable.Select<int, bool>(Observable.WithLatestFrom<Unit, int, int>(UnityUIComponentExtensions.OnClickAsObservable(purchaseButton), (IObservable<int>)_currencyProcessor.GetCountReactiveProperty(price.Currency, price.CompositeIdentificator), (Func<Unit, int, int>)((Unit _, int _count) => _count)), (Func<int, bool>)((int _) => CurrencyProcessor.IsEnough(cost, price.CompositeIdentificator))));
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<bool>(Observable.Where<bool>((IObservable<bool>)obj, (Func<bool, bool>)((bool _isEnough) => _isEnough)), (Action<bool>)delegate
		{
			SendPurchaseRequest(wholesale);
		}), (ICollection<IDisposable>)_disposables);
		DisposableExtensions.AddTo<IDisposable>(obj.Connect(), (ICollection<IDisposable>)_disposables);
	}

	private void SendPurchaseRequest(bool wholesale)
	{
		((SummonLot)base.Source).Setup(wholesale);
		Purchase();
		Set(base.Source);
	}
}
