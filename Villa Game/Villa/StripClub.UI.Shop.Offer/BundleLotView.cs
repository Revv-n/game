using System;
using System.Collections.Generic;
using GreenT.HornyScapes.DebugInfo;
using GreenT.HornyScapes.Events.Content;
using GreenT.HornyScapes.Monetization;
using GreenT.HornyScapes.Sellouts.Views;
using GreenT.Localizations;
using StripClub.Model;
using StripClub.Model.Shop;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace StripClub.UI.Shop.Offer;

public class BundleLotView : LotView
{
	[SerializeField]
	private LocalizedTextMeshPro[] bundleName;

	[SerializeField]
	private LocalizedTextMeshPro[] bundleDescription;

	[SerializeField]
	private Transform backgroundHolder;

	[SerializeField]
	private GameObject timerDescription;

	[SerializeField]
	private PriceButtonView priceTextView;

	[SerializeField]
	private Button buyButton;

	[SerializeField]
	private BundleLotFeaturesView lotFeaturesView;

	[SerializeField]
	private SaleFeatureView saleFeatureView;

	[SerializeField]
	private LootboxContentView lootboxContentView;

	[SerializeField]
	private SelloutPointsView _selloutPointsView;

	[SerializeField]
	private DebugInfoContainer _debugInfo;

	private Background offerBackground;

	private Purchaser purchaser;

	private LocalizationService _localizationService;

	private ContentSelectorGroup _contentSelectorGroup;

	private bool _isPurchasing;

	private readonly CompositeDisposable onBuyStream = new CompositeDisposable();

	[Inject]
	public void Init(Purchaser purchaser, LocalizationService localizationService, ContentSelectorGroup contentSelectorGroup)
	{
		this.purchaser = purchaser;
		_localizationService = localizationService;
		_contentSelectorGroup = contentSelectorGroup;
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
		SetupButton(bundleLot);
		SetupTextField(bundleName, bundleLot.NameKey);
		SetupTextField(bundleDescription, bundleLot.DescriptionKey);
		SetSelloutLot(bundleLot);
		TryDebug(base.Source);
	}

	protected void SetupTextField(IEnumerable<LocalizedTextMeshPro> bundleName, string key)
	{
		foreach (LocalizedTextMeshPro item in bundleName)
		{
			item.Init(key);
		}
	}

	private void SetupButton(BundleLot shopBundleLot)
	{
		priceTextView.Set(shopBundleLot.Price, shopBundleLot.OldPrice);
		buyButton.interactable = shopBundleLot.IsAvailable();
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

	private void SetSelloutLot(BundleLot bundleLot)
	{
		_selloutPointsView.SetLot(bundleLot);
		_selloutPointsView.CheckSellout();
	}

	public override void Purchase()
	{
		buyButton.interactable = false;
		BundleLot bundleLot = (BundleLot)base.Source;
		string nameForPopup = GetNameForPopup(bundleLot);
		string descriptionForPopup = GetDescriptionForPopup(bundleLot);
		onBuyStream?.Clear();
		_isPurchasing = true;
		purchaser.OnResult.Subscribe(OnPurchaseEnded).AddTo(onBuyStream);
		purchaser.TryPurchase(bundleLot, bundleLot.PaymentID, nameForPopup, descriptionForPopup, bundleLot.Data.ImageNameKey);
	}

	private void OnPurchaseEnded(bool success)
	{
		if (success)
		{
			ViewUpdateSignal signal = new ViewUpdateSignal(this);
			signalBus.Fire(signal);
			if (_isPurchasing)
			{
				_selloutPointsView.OnPurchase(base.Source.Content.AnalyticData.SourceType, _contentSelectorGroup.Current);
			}
		}
		_isPurchasing = false;
		SetupButton((BundleLot)base.Source);
	}

	private string GetDescriptionForPopup(BundleLot bundleLot)
	{
		_ = string.Empty;
		return GetLocalization(bundleLot.Data.ItemDescriptionKey);
	}

	private string GetNameForPopup(BundleLot bundleLot)
	{
		return string.Empty;
	}

	private void TryDebug(Lot lot)
	{
	}

	private string GetLocalization(string key)
	{
		return _localizationService.Text(key);
	}

	private void OnDestroy()
	{
		onBuyStream.Dispose();
	}
}
