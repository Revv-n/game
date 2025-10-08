using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Events.Content;
using GreenT.HornyScapes.Monetization;
using GreenT.HornyScapes.Sellouts.Views;
using GreenT.HornyScapes.UI;
using GreenT.Localizations;
using StripClub.Model;
using StripClub.Model.Shop;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace StripClub.UI.Shop;

public class GemLotView : LotView
{
	private const string hotStickerKey = "ui.shop.stickers.hot";

	private const string bestStickerKey = "ui.shop.stickers.best";

	private const string claimKey = "ui.shop.gem.claim";

	[SerializeField]
	protected Image lotIcon;

	[SerializeField]
	protected TMP_Text lotInfo;

	[SerializeField]
	private Image rewardIcon;

	[SerializeField]
	private TMP_Text rewardCount;

	[SerializeField]
	private TMP_Text extraRewardCount;

	[Space]
	[SerializeField]
	private TMP_Text price;

	[SerializeField]
	private TMP_Text oldPrice;

	[SerializeField]
	private GameObject oldPriceObject;

	[SerializeField]
	private SelloutPointsView _selloutPointsView;

	[Space]
	[SerializeField]
	private GameObject sale;

	[SerializeField]
	private GameObject saleTextContainer;

	[SerializeField]
	private LocalizedTextMeshPro saleValueText;

	[SerializeField]
	private GameObject firstPurchaseTextContainer;

	[SerializeField]
	private GameObject sticker;

	[SerializeField]
	private GameObject textStickerObject;

	[SerializeField]
	private TMP_Text textSticker;

	[Space]
	[SerializeField]
	private Button buyButton;

	[SerializeField]
	private AddResourcesAnimation addResourcesAnimation;

	private LocalizationService _localizationService;

	private ContentSelectorGroup _contentSelectorGroup;

	private bool _isPurchasing;

	private IDisposable _infoDisposable;

	private IDisposable _stickerDisposable;

	protected Purchaser purchaser;

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
		GemShopLot gemShopLot = (GemShopLot)lot;
		_infoDisposable?.Dispose();
		_infoDisposable = ObservableExtensions.Subscribe<string>((IObservable<string>)_localizationService.ObservableText(lot.LocalizationKey), (Action<string>)delegate(string text)
		{
			lotInfo.text = text;
		});
		SubscribeBuyBtn(gemShopLot);
		SetIcon(gemShopLot);
		SetReward(gemShopLot);
		SetPrice(gemShopLot);
		SetStickers(gemShopLot);
		SetSelloutLot(gemShopLot);
		CurrencyLinkedContent next = base.Source.Content.GetNext<CurrencyLinkedContent>(checkThis: true);
		if (next != null)
		{
			addResourcesAnimation.Init(next);
		}
	}

	private void SubscribeBuyBtn(GemShopLot lot)
	{
		if (lot.Price.Currency.IsRealCurrency() || !base.Source.IsFree)
		{
			buyButton.onClick.RemoveListener(Purchase);
			buyButton.onClick.AddListener(Purchase);
		}
	}

	public override void Purchase()
	{
		GemShopLot gemShopLot = (GemShopLot)base.Source;
		string nameForPopup = GetNameForPopup(gemShopLot);
		string descriptionForPopup = GetDescriptionForPopup(gemShopLot);
		CompositeDisposable obj = onBuyStream;
		if (obj != null)
		{
			obj.Clear();
		}
		_isPurchasing = true;
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<bool>(purchaser.OnResult, (Action<bool>)delegate(bool result)
		{
			OnPurchaseEnded(result);
			onBuyStream.Clear();
		}), (ICollection<IDisposable>)onBuyStream);
		purchaser.TryPurchase(gemShopLot, gemShopLot.PaymentID, nameForPopup, descriptionForPopup, gemShopLot.Data.ImageNameKey);
	}

	private void OnPurchaseEnded(bool success)
	{
		if (success)
		{
			ViewUpdateSignal viewUpdateSignal = new ViewUpdateSignal(this);
			signalBus.Fire<ViewUpdateSignal>(viewUpdateSignal);
			if (_isPurchasing)
			{
				_selloutPointsView.OnPurchase(base.Source.Content.AnalyticData.SourceType, _contentSelectorGroup.Current);
			}
		}
		_isPurchasing = false;
	}

	private void SetIcon(GemShopLot gemshopLot)
	{
		Sprite icon = gemshopLot.GetIcon();
		_ = icon == null;
		lotIcon.sprite = icon;
	}

	private void SetReward(GemShopLot lot)
	{
		rewardIcon.sprite = lot.Reward.GetIcon();
		rewardCount.text = lot.Reward.GetDescription();
		extraRewardCount.gameObject.SetActive(lot.ExtraReward != null);
		if (lot.ExtraReward != null)
		{
			extraRewardCount.text = "+" + lot.ExtraReward.GetDescription();
		}
	}

	private void SetPrice(GemShopLot lot)
	{
		price.text = ((lot.Price.Value > 0m) ? lot.Price.ToString() : _localizationService.Text("ui.shop.gem.claim"));
		oldPriceObject.SetActive(lot.OldPrice != null);
		oldPrice.text = lot.OldPrice?.Value.ToString() ?? string.Empty;
	}

	private void SetStickers(GemShopLot lot)
	{
		bool flag = (lot.Stickers & Stickers.Sale) == Stickers.Sale;
		bool flag2 = (lot.Stickers & Stickers.FirstPurchase) == Stickers.FirstPurchase;
		bool flag3 = (lot.Stickers & Stickers.Hot) == Stickers.Hot;
		bool flag4 = (lot.Stickers & Stickers.Best) == Stickers.Best;
		sale.SetActive(flag);
		sticker.SetActive(flag2 || flag);
		saleTextContainer.SetActive(flag);
		if (lot.SaleValue.HasValue)
		{
			saleValueText.SetArguments(lot.SaleValue);
		}
		firstPurchaseTextContainer.SetActive(flag2 && !flag);
		textStickerObject.SetActive(flag3 || flag4);
		_stickerDisposable?.Dispose();
		string text2 = (flag3 ? "ui.shop.stickers.hot" : (flag4 ? "ui.shop.stickers.best" : null));
		if (!string.IsNullOrEmpty(text2))
		{
			_stickerDisposable = ObservableExtensions.Subscribe<string>((IObservable<string>)_localizationService.ObservableText(text2), (Action<string>)delegate(string text)
			{
				textSticker.text = text;
			});
		}
		else
		{
			textSticker.text = string.Empty;
		}
	}

	private void SetSelloutLot(GemShopLot bundleLot)
	{
		_selloutPointsView.SetLot(bundleLot);
		_selloutPointsView.CheckSellout();
	}

	private string GetDescriptionForPopup(GemShopLot bundleLot)
	{
		_ = string.Empty;
		return GetLocalization(bundleLot.Data.ItemDescriptionKey);
	}

	private string GetNameForPopup(GemShopLot bundleLot)
	{
		return string.Empty;
	}

	private string GetLocalization(string key)
	{
		return _localizationService.Text(key);
	}

	private void OnDisable()
	{
		_stickerDisposable?.Dispose();
		_infoDisposable?.Dispose();
	}

	private void OnDestroy()
	{
		onBuyStream.Dispose();
		_stickerDisposable?.Dispose();
		_infoDisposable?.Dispose();
	}
}
