using System;
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

	public IObservable<PaymentMethod> OnPaymentSelected => _onPaymentSelected.AsObservable();

	public IObservable<Unit> OnExitClicked => _onExitClicked.AsObservable();

	public override void Init(IWindowsManager windowsOpener)
	{
		base.Canvas = GetComponentInParent<Canvas>();
		base.Init(windowsOpener);
	}

	protected override void Awake()
	{
		base.Awake();
		_compositeDisposable?.Clear();
		_nextButton.OnClickAsObservable().Subscribe(delegate
		{
			GoNext();
		}).AddTo(_compositeDisposable);
		_exitButton.OnClickAsObservable().Subscribe(delegate
		{
			_onExitClicked.OnNext(Unit.Default);
		}).AddTo(_compositeDisposable);
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
		_compositeDisposable?.Dispose();
	}
}
