using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Monetization;
using StripClub.Model;
using StripClub.Model.Shop;
using StripClub.UI;
using StripClub.UI.Shop;
using StripClub.UI.Shop.Offer;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Bank.UI;

public class FirstPurchaseLotView : LotView
{
	private const string getString = "ui.bank.first_purchase.button.get";

	private const string purchaseString = "ui.bank.first_purchase.button.purchase";

	[SerializeField]
	private LocalizedTextMeshPro[] bundleName;

	[SerializeField]
	private LocalizedTextMeshPro[] subtitle;

	[SerializeField]
	private Transform backgroundHolder;

	[SerializeField]
	private Button buyButton;

	[SerializeField]
	private BundleLotFeaturesView lotFeaturesView;

	[SerializeField]
	private SaleFeatureView saleFeatureView;

	[SerializeField]
	private LootboxContentView lootboxContentView;

	[SerializeField]
	private OpenSection sectionOpener;

	[SerializeField]
	private LocalizedTextMeshPro buttonText;

	[SerializeField]
	private Image attentionIcon;

	private Background offerBackground;

	private IDisposable bundleLockerStream;

	private IDisposable onBuyStream;

	private Purchaser purchaser;

	[Inject]
	private void Init(Purchaser purchaser)
	{
		this.purchaser = purchaser;
	}

	public override void Set(Lot lot)
	{
		base.Set(lot);
		if (!(lot is BundleLot bundleLot))
		{
			throw new InvalidCastException("Lot (ID:" + lot.ID + ") of type: " + lot.GetType().ToString() + " can't be casted to " + typeof(BundleLot).ToString());
		}
		lotFeaturesView.Set(bundleLot.Features);
		saleFeatureView.Set(bundleLot.Features);
		DisplayContent(bundleLot);
		SetupPresetData(bundleLot);
		SetupTextField(bundleName, bundleLot.NameKey);
		SetupTextField(subtitle, bundleLot.DescriptionKey);
		bundleLockerStream = lot.Locker.IsOpen.Subscribe(SetupButton);
	}

	private void Awake()
	{
		SubscribeOnButton();
	}

	protected void SetupTextField(IEnumerable<LocalizedTextMeshPro> bundleName, string key)
	{
		foreach (LocalizedTextMeshPro item in bundleName)
		{
			item.Init(key);
		}
	}

	private void SubscribeOnButton()
	{
		buyButton.onClick.AddListener(Purchase);
	}

	private void SetupButton(bool available)
	{
		buttonText.Init(available ? "ui.bank.first_purchase.button.get" : "ui.bank.first_purchase.button.purchase");
		attentionIcon.gameObject.SetActive(available);
	}

	private void DisplayContent(BundleLot shopBundleLot)
	{
		if (shopBundleLot.Content is LootboxLinkedContent content)
		{
			lootboxContentView.SetGuarantedRewardsWithViewSettings(content, shopBundleLot.Settings);
		}
	}

	private void SetupPresetData(BundleLot shopBundleLot)
	{
		if (offerBackground != null)
		{
			UnityEngine.Object.Destroy(offerBackground.gameObject);
		}
		GameObject gameObject = shopBundleLot.Settings.GetBackground();
		_ = gameObject == null;
		gameObject = UnityEngine.Object.Instantiate(gameObject, backgroundHolder);
		offerBackground = gameObject.GetComponent<Background>();
		if (!(offerBackground == null) && shopBundleLot.Settings.IsCharacterSpriteOverridden)
		{
			Sprite girlSprite = shopBundleLot.Settings.GetGirlSprite();
			if (!(girlSprite == null))
			{
				offerBackground.Setup(girlSprite);
			}
		}
	}

	public override void Purchase()
	{
		if (base.Source.Locker.IsOpen.Value)
		{
			BundleLot bundleLot = (BundleLot)base.Source;
			onBuyStream?.Dispose();
			IObservable<bool> onResult = purchaser.OnResult;
			buyButton.interactable = false;
			onBuyStream = onResult.Subscribe(OnPurchaseEnded);
			purchaser.TryPurchase(bundleLot, string.Empty, bundleLot.NameKey);
		}
		else
		{
			sectionOpener.Open();
		}
	}

	private void OnPurchaseEnded(bool success)
	{
		if (success)
		{
			ViewUpdateSignal signal = new ViewUpdateSignal(this);
			signalBus.Fire(signal);
		}
	}

	protected virtual void OnDestroy()
	{
		buyButton.onClick.RemoveListener(Purchase);
		bundleLockerStream?.Dispose();
		onBuyStream?.Dispose();
	}
}
