using GreenT.AssetBundles;
using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.Characters;
using GreenT.HornyScapes.Characters.Skins;
using GreenT.HornyScapes.Characters.Skins.Content;
using GreenT.HornyScapes.Content;
using GreenT.HornyScapes.Events.Content;
using GreenT.HornyScapes.Monetization;
using GreenT.HornyScapes.Sellouts.Views;
using GreenT.Localizations;
using GreenT.Types;
using StripClub.Model;
using StripClub.Model.Cards;
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

public sealed class MiniEvent8SlotsLotView : LotView
{
	[SerializeField]
	private Image _lotIcon;

	[Space]
	[SerializeField]
	private StatableComponent _characterBackground;

	[SerializeField]
	private GameObject _defaultBackground;

	[SerializeField]
	private Image _characterIcon;

	[SerializeField]
	private GameObject _defaultIcon;

	[SerializeField]
	private StatableComponent _characterDescriptionStatable;

	[SerializeField]
	private GameObject _characterDescriptionRoot;

	[SerializeField]
	private LocalizedTextMeshPro _characterName;

	[SerializeField]
	private LocalizedTextMeshPro _characterDescription;

	[SerializeField]
	private StatableComponent _rays;

	[Space]
	[SerializeField]
	private TMP_Text _quantity;

	[SerializeField]
	private TMP_Text _price;

	[SerializeField]
	private TMP_Text _oldPrice;

	[SerializeField]
	private GameObject _oldPriceObject;

	[SerializeField]
	private Image _oldPriceCurrencyIcon;

	[SerializeField]
	private Image _priceCurrencyIcon;

	[SerializeField]
	private StatableComponent _priceColorStatable;

	[Space]
	[SerializeField]
	private GameObject _sale;

	[SerializeField]
	private GameObject _saleTextContainer;

	[SerializeField]
	private LocalizedTextMeshPro _saleValueText;

	[SerializeField]
	private GameObject _firstPurchaseTextContainer;

	[SerializeField]
	private GameObject _sticker;

	[SerializeField]
	private GameObject _textStickerObject;

	[SerializeField]
	private TMP_Text _textSticker;

	[Space]
	[SerializeField]
	private Button _buyButton;

	[Space]
	[SerializeField]
	private SelloutPointsView _selloutPointsView;

	private ICurrencyProcessor _currencyProcessor;

	private GameSettings _gameSettings;

	private Purchaser _purchaser;

	private LocalizationService _localizationService;

	private CompositeDisposable _onBuyStream = new CompositeDisposable();

	private CompositeDisposable _disposables = new CompositeDisposable();

	private CurrencyAmplitudeAnalytic _currencyAmplitudeAnalytic;

	private FakeAssetService _fakeAssetService;

	private ContentSelectorGroup _contentSelectorGroup;

	private const string HOT_STICKER_KEY = "ui.shop.stickers.hot";

	private const string BEST_STICKER_KEY = "ui.shop.stickers.best";

	private const string CLAIM_KEY = "ui.shop.gem.claim";

	private const string DESCRIPTION_GIRL_KEY = "ui.8slots";

	private const string DESCRIPTION_SKIN_KEY = ".skin";

	private const string DESCRIPTION_CARD_KEY = ".card";

	private void OnDestroy()
	{
		_onBuyStream.Dispose();
	}

	[Inject]
	public void Init(Purchaser purchaser, LocalizationService localizationService, GameSettings gameSettings, ICurrencyProcessor currencyProcessor, CurrencyAmplitudeAnalytic currencyAmplitudeAnalytic, FakeAssetService fakeAssetService, ContentSelectorGroup contentSelectorGroup)
	{
		_purchaser = purchaser;
		_localizationService = localizationService;
		_gameSettings = gameSettings;
		_currencyProcessor = currencyProcessor;
		_currencyAmplitudeAnalytic = currencyAmplitudeAnalytic;
		_fakeAssetService = fakeAssetService;
		_contentSelectorGroup = contentSelectorGroup;
	}

	public override void Set(Lot lot)
	{
		base.Set(lot);
		GemShopLot gemShopLot = (GemShopLot)lot;
		SubscribeBuyBtn(gemShopLot);
		SetIcon(gemShopLot);
		SetQuantity(gemShopLot);
		SetReward(gemShopLot);
		SetPrice(gemShopLot);
		SetStickers(gemShopLot);
		SetSelloutLot(gemShopLot);
	}

	private void SubscribeBuyBtn(GemShopLot lot)
	{
		if (lot.Price.Currency.IsRealCurrency() || !base.Source.IsFree)
		{
			_buyButton.onClick.RemoveListener(Purchase);
			_buyButton.onClick.AddListener(Purchase);
		}
	}

	public override void Purchase()
	{
		GemShopLot gemShopLot = (GemShopLot)base.Source;
		string nameForPopup = GetNameForPopup(gemShopLot);
		string descriptionForPopup = GetDescriptionForPopup(gemShopLot);
		_onBuyStream?.Clear();
		_purchaser.OnResult.Subscribe(OnPurchaseEnded).AddTo(_onBuyStream);
		_purchaser.TryPurchase(gemShopLot, gemShopLot.PaymentID, nameForPopup, descriptionForPopup, gemShopLot.Data.ImageNameKey);
	}

	private void OnPurchaseEnded(bool success)
	{
		if (success)
		{
			_onBuyStream.Clear();
			GemShopLot gemShopLot = (GemShopLot)base.Source;
			_currencyAmplitudeAnalytic.SendSpentEvent(gemShopLot.Price.Currency, (int)gemShopLot.Price.Value, CurrencyAmplitudeAnalytic.SourceType.EightSlotsMinievents, ContentType.Event, gemShopLot.Price.CompositeIdentificator);
			ViewUpdateSignal signal = new ViewUpdateSignal(this);
			signalBus.Fire(signal);
			_selloutPointsView.OnPurchase(base.Source.Content.AnalyticData.SourceType, _contentSelectorGroup.Current);
		}
	}

	private void SetIcon(GemShopLot gemshopLot)
	{
		LinkedContent reward = gemshopLot.Reward;
		SkinLinkedContent skinLinkedContent = reward as SkinLinkedContent;
		if (skinLinkedContent == null)
		{
			CardLinkedContent cardLinkedContent = reward as CardLinkedContent;
			if (cardLinkedContent == null)
			{
				if (reward is CurrencyLinkedContent)
				{
					_lotIcon.sprite = gemshopLot.GetIcon();
				}
				else
				{
					_lotIcon.sprite = gemshopLot.Reward.GetIcon();
				}
			}
			else
			{
				_fakeAssetService.SetFakeCharacterBankImages(cardLinkedContent.Card, _characterIcon, (ICharacter character) => cardLinkedContent.GetFullIcon());
			}
		}
		else
		{
			_fakeAssetService.SetFakeSkinIcon(skinLinkedContent.Skin, _characterIcon, (Skin character) => skinLinkedContent.GetFullIcon());
		}
	}

	private void SetQuantity(GemShopLot gemShopLot)
	{
		Transform parent = _quantity.gameObject.transform.parent;
		if (gemShopLot.Reward is CurrencyLinkedContent)
		{
			parent.gameObject.SetActive(value: true);
			_quantity.text = gemShopLot.Reward.GetDescription();
		}
		else
		{
			parent.gameObject.SetActive(value: false);
		}
	}

	private void SetReward(GemShopLot lot)
	{
		bool flag = lot.Reward is SkinLinkedContent || lot.Reward is CardLinkedContent;
		bool flag2 = lot.Reward is LootboxLinkedContent;
		bool flag3 = lot.Reward is MergeItemLinkedContent mergeItemLinkedContent && mergeItemLinkedContent.GameItemConfig.Key.Collection == "EnergyChest";
		_defaultBackground.SetActive(!flag);
		_characterBackground.gameObject.SetActive(flag);
		_defaultIcon.SetActive(!flag);
		_characterIcon.gameObject.SetActive(flag);
		_characterDescriptionRoot.SetActive(flag);
		_characterBackground.Set((int)lot.Reward.GetRarity());
		_characterDescriptionStatable.Set((int)lot.Reward.GetRarity());
		string text = "ui.8slots";
		if (flag)
		{
			text += $".{lot.Reward.GetRarity()}";
			text += ((lot.Reward is SkinLinkedContent) ? ".skin" : ".card");
		}
		_characterDescription.Init(text);
		_characterName.Init(lot.LocalizationKey);
		_rays.gameObject.SetActive(flag2 || flag3);
		if (flag2)
		{
			if (lot.Reward.GetRarity() != Rarity.Rare)
			{
				_rays.Set((int)lot.Reward.GetRarity());
			}
			else
			{
				_rays.gameObject.SetActive(value: false);
			}
		}
		if (flag3)
		{
			_rays.Set(3);
		}
	}

	private void SetPrice(GemShopLot lot)
	{
		_price.text = ((lot.Price.Value > 0m) ? lot.Price.ToString() : _localizationService.Text("ui.shop.gem.claim"));
		_oldPriceObject.SetActive(lot.OldPrice != null);
		_oldPrice.text = lot.OldPrice?.Value.ToString() ?? string.Empty;
		bool flag = lot.Price.Currency.IsRealCurrency();
		_priceCurrencyIcon.gameObject.SetActive(!flag);
		_oldPriceCurrencyIcon.gameObject.SetActive(!flag);
		if (!flag)
		{
			if (_gameSettings.CurrencySettings.TryGetValue(lot.Price.Currency, out var currencySettings, lot.Price.CompositeIdentificator))
			{
				_priceCurrencyIcon.sprite = currencySettings.Sprite;
			}
			if (lot.OldPrice != null && _gameSettings.CurrencySettings.TryGetValue(lot.OldPrice.Currency, out var currencySettings2, lot.OldPrice.CompositeIdentificator))
			{
				_oldPriceCurrencyIcon.sprite = currencySettings2.Sprite;
			}
			Cost cost = new Cost((int)lot.Price.Value, lot.Price.Currency);
			(from _ in _currencyProcessor.GetCountReactiveProperty(lot.Price.Currency, lot.Price.CompositeIdentificator)
				select CurrencyProcessor.IsEnough(cost, lot.Price.CompositeIdentificator) ? 1 : 0).Subscribe(_priceColorStatable.Set).AddTo(_disposables);
		}
	}

	private void SetStickers(GemShopLot lot)
	{
		bool flag = (lot.Stickers & Stickers.Sale) == Stickers.Sale;
		bool flag2 = (lot.Stickers & Stickers.FirstPurchase) == Stickers.FirstPurchase;
		bool flag3 = (lot.Stickers & Stickers.Hot) == Stickers.Hot;
		bool flag4 = (lot.Stickers & Stickers.Best) == Stickers.Best;
		_sale.SetActive(flag);
		_sticker.SetActive(flag2 || flag);
		_saleTextContainer.SetActive(flag);
		if (lot.SaleValue.HasValue)
		{
			_saleValueText.SetArguments(lot.SaleValue);
		}
		_firstPurchaseTextContainer.SetActive(flag2 && !flag);
		_textStickerObject.SetActive(flag3 || flag4);
		_textSticker.text = (flag3 ? _localizationService.Text("ui.shop.stickers.hot") : (flag4 ? _localizationService.Text("ui.shop.stickers.best") : string.Empty));
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
}
