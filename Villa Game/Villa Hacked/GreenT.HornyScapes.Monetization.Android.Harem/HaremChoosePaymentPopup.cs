using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Animations;
using GreenT.UI;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Monetization.Android.Harem;

public class HaremChoosePaymentPopup : PopupWindow
{
	[SerializeField]
	private Button _exitButton;

	[SerializeField]
	private Button _nextButton;

	[SerializeField]
	private Image _imageItem;

	[SerializeField]
	private TextMeshProUGUI _textPurchaseName;

	[SerializeField]
	private TextMeshProUGUI _textPrice;

	[SerializeField]
	private Toggle _toogleCard;

	[SerializeField]
	private Toggle _toogleCrypto;

	private readonly Subject<PaymentMethod> _onPaymentSelected = new Subject<PaymentMethod>();

	private readonly Subject<Unit> _onExitClicked = new Subject<Unit>();

	private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

	public IObservable<PaymentMethod> OnPaymentSelected => Observable.AsObservable<PaymentMethod>((IObservable<PaymentMethod>)_onPaymentSelected);

	public IObservable<Unit> OnExitClicked => Observable.AsObservable<Unit>((IObservable<Unit>)_onExitClicked);

	public override void Init(IWindowsManager windowsOpener)
	{
		base.Canvas = GetComponentInParent<Canvas>();
		base.Init(windowsOpener);
	}

	protected override void Awake()
	{
		base.Awake();
		CompositeDisposable compositeDisposable = _compositeDisposable;
		if (compositeDisposable != null)
		{
			compositeDisposable.Clear();
		}
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Unit>(UnityUIComponentExtensions.OnClickAsObservable(_nextButton), (Action<Unit>)delegate
		{
			GoNext();
		}), (ICollection<IDisposable>)_compositeDisposable);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Unit>(UnityUIComponentExtensions.OnClickAsObservable(_exitButton), (Action<Unit>)delegate
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			_onExitClicked.OnNext(Unit.Default);
		}), (ICollection<IDisposable>)_compositeDisposable);
	}

	public void ShowItem(string namePurchase, string price, Sprite sprite, bool isCryptoAvailable = false)
	{
		if (sprite == null)
		{
			_imageItem.gameObject.SetActive(value: false);
		}
		else
		{
			_imageItem.gameObject.SetActive(value: true);
			_imageItem.sprite = sprite;
		}
		_textPurchaseName.text = namePurchase;
		_textPrice.text = price;
		_toogleCrypto.gameObject.SetActive(isCryptoAvailable);
		_toogleCard.isOn = true;
	}

	private void GoNext()
	{
		if (_toogleCard.isOn)
		{
			_onPaymentSelected.OnNext(PaymentMethod.Centrobill);
		}
		else
		{
			_onPaymentSelected.OnNext(PaymentMethod.Bitcoin);
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		CompositeDisposable compositeDisposable = _compositeDisposable;
		if (compositeDisposable != null)
		{
			compositeDisposable.Dispose();
		}
	}
}
