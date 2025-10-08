using System;
using Erolabs.Sdk.Unity;
using GreenT.HornyScapes.Erolabs;
using Merge;
using StripClub.UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Monetization.Android.Erolabs;

public class ErolabsPopup : MonetizationPopup
{
	[SerializeField]
	private string notEnoughMoneyDescriptionLocalizeKey;

	[SerializeField]
	private string guestDescriptionLocalizeKey;

	[SerializeField]
	private string erolabsBalanceLocalizeKey;

	[SerializeField]
	private string erolabsBindingLocalizeKey;

	[SerializeField]
	private string erolabsBindCompletedLocalizeKey;

	[SerializeField]
	private string headerPaymentLocalizeKey;

	[SerializeField]
	private string headerBindingLocalizeKey;

	[SerializeField]
	private Button BuyEcoinButton;

	[SerializeField]
	private Button BindButton;

	[SerializeField]
	private Button BuyButton;

	[SerializeField]
	private Image erolabsEcoinImage;

	[SerializeField]
	private LocalizedTextMeshPro header;

	private int currentBalance;

	private Action<Unit> currentCallback;

	private ErolabsSDKAuthorization erolabsSDKAuthorization;

	[Inject]
	public void Construct(ErolabsSDKAuthorization erolabsSDKAuthorization)
	{
		this.erolabsSDKAuthorization = erolabsSDKAuthorization;
	}

	protected override void Awake()
	{
		base.Awake();
		SetupListeners();
	}

	private void SetupListeners()
	{
		SupportButton.onClick.AddListener(supportUrlOpener.OpenUrl);
		BuyEcoinButton.onClick.AddListener(RedirectToBuyECoin);
		AbortButton.onClick.AddListener(Close);
		BindButton.onClick.AddListener(ShowBindingView);
	}

	public void ShowNotEnoughMoneyView()
	{
		UpdateView(notEnoughMoneyDescriptionLocalizeKey, headerPaymentLocalizeKey, showBuyEcoin: true, showBind: false, showSupport: false, showBuy: false, showAbort: false, showClose: true, showLoader: false, showFinalImage: true, showErolabsEcoin: false, null);
	}

	public void ShowGuestView()
	{
		UpdateView(guestDescriptionLocalizeKey, headerPaymentLocalizeKey, showBuyEcoin: false, showBind: true, showSupport: false, showBuy: false, showAbort: false, showClose: true, showLoader: false, showFinalImage: true, showErolabsEcoin: false, null);
	}

	public void ShowCheckBalanceView(int balance, Action<Unit> callback)
	{
		currentBalance = balance;
		currentCallback = callback;
		UpdateView(erolabsBalanceLocalizeKey, headerPaymentLocalizeKey, showBuyEcoin: false, showBind: false, showSupport: false, showBuy: true, showAbort: false, showClose: true, showLoader: false, showFinalImage: false, showErolabsEcoin: true, delegate
		{
			currentCallback?.Invoke(default(Unit));
		});
		Description.Text.text = string.Format(Description.Text.text, currentBalance);
	}

	public void ShowBindingView()
	{
		UpdateView(erolabsBindingLocalizeKey, headerBindingLocalizeKey, showBuyEcoin: false, showBind: false, showSupport: false, showBuy: false, showAbort: true, showClose: false, showLoader: true, showFinalImage: false, showErolabsEcoin: false, null);
		erolabsSDKAuthorization.BindAccount(ShowBindComplete);
	}

	public void ShowBindComplete()
	{
		UpdateView(erolabsBindCompletedLocalizeKey, headerBindingLocalizeKey, showBuyEcoin: false, showBind: false, showSupport: false, showBuy: false, showAbort: false, showClose: true, showLoader: false, showFinalImage: false, showErolabsEcoin: true, null);
	}

	private void UpdateView(string descriptionKey, string headerKey, bool showBuyEcoin, bool showBind, bool showSupport, bool showBuy, bool showAbort, bool showClose, bool showLoader, bool showFinalImage, bool showErolabsEcoin, Action buyButtonAction)
	{
		SetDescriptionLocalization(descriptionKey);
		header.Init(headerKey);
		ConfigureButtons(showBuyEcoin, showBind, showSupport, showBuy, showAbort, showClose);
		ConfigureImages(showLoader, showFinalImage, showErolabsEcoin);
		BuyButton.onClick.RemoveAllListeners();
		if (buyButtonAction != null)
		{
			BuyButton.onClick.AddListener(buyButtonAction.Invoke);
		}
	}

	private void ConfigureButtons(bool showBuyEcoin, bool showBind, bool showSupport, bool showBuy, bool showAbort, bool showClose)
	{
		BuyEcoinButton.SetActive(showBuyEcoin);
		BindButton.SetActive(showBind);
		SupportButton.SetActive(showSupport);
		BuyButton.SetActive(showBuy);
		AbortButton.gameObject.SetActive(showAbort);
		CloseButton.gameObject.SetActive(showClose);
	}

	private void ConfigureImages(bool showLoader, bool showFinalImage, bool showErolabsEcoin)
	{
		Loader.gameObject.SetActive(showLoader);
		FinalImage.gameObject.SetActive(showFinalImage);
		erolabsEcoinImage.gameObject.SetActive(showErolabsEcoin);
	}

	private void RedirectToBuyECoin()
	{
		ErolabsSDK.OpenPayment();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		ClearListeners();
	}

	private void ClearListeners()
	{
		AbortButton?.onClick.RemoveAllListeners();
		BindButton?.onClick.RemoveAllListeners();
		SupportButton?.onClick.RemoveAllListeners();
		BuyEcoinButton?.onClick.RemoveAllListeners();
		BuyButton?.onClick.RemoveAllListeners();
	}
}
