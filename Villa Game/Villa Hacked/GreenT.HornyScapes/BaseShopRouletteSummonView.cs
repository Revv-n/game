using System;
using System.Collections.Generic;
using GreenT.Localizations;
using StripClub.Model.Data;
using StripClub.Model.Shop;
using StripClub.UI.Shop;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes;

public abstract class BaseShopRouletteSummonView<T> : RouletteLotBackgroundView where T : RouletteSummonLot
{
	[SerializeField]
	private TextMeshProUGUI[] _titleTextFields;

	[SerializeField]
	private TextMeshProUGUI[] _descriptionTextFields;

	[SerializeField]
	private TextMeshProUGUI _guaranteedDescriptionTextField;

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
	private MultiContentView _rouletteContentView;

	[Space]
	[SerializeField]
	protected LotRedirection _lotRedirection;

	private CompositeDisposable _disposables = new CompositeDisposable();

	private LocalizationService _localization;

	public ReactiveProperty<T> OnSet = new ReactiveProperty<T>();

	[Inject]
	public void Init(ICurrencyProcessor currencyProcessor, LocalizationService localization)
	{
		_currencyProcessor = currencyProcessor;
		_localization = localization;
	}

	protected virtual void OnEnable()
	{
		if (base.Source != null)
		{
			Set((T)base.Source);
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

	public override void Set(RouletteLot lot)
	{
		_disposables.Clear();
		base.Set(lot);
		T val = (T)lot;
		TextMeshProUGUI[] titleTextFields = _titleTextFields;
		for (int i = 0; i < titleTextFields.Length; i++)
		{
			titleTextFields[i].text = _localization.Text(val.TitleKey);
		}
		titleTextFields = _descriptionTextFields;
		for (int i = 0; i < titleTextFields.Length; i++)
		{
			titleTextFields[i].text = _localization.Text(val.DescriptionKey);
		}
		_guaranteedDescriptionTextField.text = _localization.Text(val.GuaranteedDescriptionKey);
		SetButtons(val);
		DisplayContent(val);
		OnSet.Value = val;
	}

	protected abstract void TryRedirect();

	private void DisplayContent(T rouletteSummonLot)
	{
		_rouletteContentView.Set(rouletteSummonLot.MainDropSettings, rouletteSummonLot.SecondaryDropSettings);
	}

	private void SetButtons(T rouletteSummonLot)
	{
		Price<int> singlePrice = rouletteSummonLot.SinglePrice;
		SetupButton(_singlePrice, singlePrice, _singlePurchaseButton, wholesale: false);
		Price<int> wholesalePrice = rouletteSummonLot.WholesalePrice;
		SetupButton(_wholesalePrice, wholesalePrice, _wholesalePurchaseButton, wholesale: true);
	}

	private void SetupButton(PriceWithFreeView view, Price<int> price, Button purchaseButton, bool wholesale)
	{
		view.Set(price);
		Cost cost = new Cost(price.Value, price.Currency);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<int>(Observable.Select<int, int>((IObservable<int>)_currencyProcessor.GetCountReactiveProperty(price.Currency, price.CompositeIdentificator), (Func<int, int>)((int _) => _currencyProcessor.IsEnough(cost, price.CompositeIdentificator) ? 1 : 0)), (Action<int>)view.SetValueColor), (ICollection<IDisposable>)_disposables);
		IConnectableObservable<bool> obj = Observable.Publish<bool>(Observable.Select<int, bool>(Observable.WithLatestFrom<Unit, int, int>(UnityUIComponentExtensions.OnClickAsObservable(purchaseButton), (IObservable<int>)_currencyProcessor.GetCountReactiveProperty(price.Currency, price.CompositeIdentificator), (Func<Unit, int, int>)((Unit _, int _count) => _count)), (Func<int, bool>)((int _) => _currencyProcessor.IsEnough(cost, price.CompositeIdentificator))));
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<bool>(Observable.Where<bool>((IObservable<bool>)obj, (Func<bool, bool>)((bool _isEnough) => _isEnough)), (Action<bool>)delegate
		{
			SendPurchaseRequest(wholesale);
		}), (ICollection<IDisposable>)_disposables);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<bool>(Observable.Where<bool>((IObservable<bool>)obj, (Func<bool, bool>)((bool _isEnough) => !_isEnough)), (Action<bool>)delegate
		{
			TryRedirect();
		}), (ICollection<IDisposable>)_disposables);
		DisposableExtensions.AddTo<IDisposable>(obj.Connect(), (ICollection<IDisposable>)_disposables);
	}

	private void SendPurchaseRequest(bool wholesale)
	{
		((T)base.Source).Setup(wholesale);
		Purchase();
		Set(base.Source);
	}
}
