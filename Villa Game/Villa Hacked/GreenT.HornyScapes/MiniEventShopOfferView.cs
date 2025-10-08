using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.Bank.UI;
using GreenT.HornyScapes.Events.Content;
using GreenT.HornyScapes.Monetization;
using GreenT.HornyScapes.Sellouts.Views;
using GreenT.Localizations;
using GreenT.Types;
using StripClub.Model;
using StripClub.Model.Shop;
using StripClub.UI;
using StripClub.UI.Shop;
using StripClub.UI.Shop.Offer;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes;

public class MiniEventShopOfferView : LotView
{
	[SerializeField]
	private LocalizedTextMeshPro[] _bundleName;

	[SerializeField]
	private LocalizedTextMeshPro[] _subtitle;

	[SerializeField]
	private Transform _backgroundHolder;

	[SerializeField]
	private Button _buyButton;

	[SerializeField]
	private PriceButtonView _priceTextView;

	[SerializeField]
	private BundleLotFeaturesView _lotFeaturesView;

	[SerializeField]
	private SaleFeatureView _saleFeatureView;

	[SerializeField]
	private LootboxContentView _lootboxContentView;

	[SerializeField]
	private OpenSection _sectionOpener;

	[SerializeField]
	private LocalizedTextMeshPro _buttonText;

	[SerializeField]
	private Image _attentionIcon;

	[SerializeField]
	private GameObject _topSticker;

	[SerializeField]
	private SelloutPointsView _selloutPointsView;

	private CurrencyAmplitudeAnalytic _currencyAmplitudeAnalytic;

	private Background _offerBackground;

	private CompositeDisposable _onBuyStream = new CompositeDisposable();

	private IMonetizationAdapter _iapPurchaser;

	private LocalizationService _localizationService;

	private ContentSelectorGroup _contentSelectorGroup;

	[Inject]
	public void Init(IMonetizationAdapter iapPurchaser, LocalizationService localizationService, CurrencyAmplitudeAnalytic currencyAmplitudeAnalytic, ContentSelectorGroup contentSelectorGroup)
	{
		_iapPurchaser = iapPurchaser;
		_localizationService = localizationService;
		_currencyAmplitudeAnalytic = currencyAmplitudeAnalytic;
		_contentSelectorGroup = contentSelectorGroup;
	}

	public override void Set(Lot lot)
	{
		base.Set(lot);
		if (!(lot is BundleLot bundleLot))
		{
			throw new InvalidCastException("Lot (ID:" + lot.ID + ") of type: " + lot.GetType().ToString() + " can't be casted to " + typeof(BundleLot).ToString());
		}
		_lotFeaturesView.Set(bundleLot.Features);
		_saleFeatureView.Set(bundleLot.Features);
		DisplayContent(bundleLot);
		SetupPresetData(bundleLot);
		SetupButton(bundleLot);
		SetupTextField(_bundleName, bundleLot.NameKey);
		SetupTextField(_subtitle, bundleLot.DescriptionKey);
		_topSticker.SetActive(value: false);
		SetSelloutLot(bundleLot);
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
		_priceTextView.Set(shopBundleLot.Price, shopBundleLot.OldPrice);
		_buyButton.interactable = shopBundleLot.IsAvailable();
	}

	private void SetSelloutLot(BundleLot bundleLot)
	{
		_selloutPointsView.SetLot(bundleLot);
		_selloutPointsView.CheckSellout();
	}

	private void DisplayContent(BundleLot shopBundleLot)
	{
		if (shopBundleLot.Content is LootboxLinkedContent content)
		{
			_lootboxContentView.SetGuarantedRewardsWithViewSettings(content, shopBundleLot.Settings);
		}
	}

	private void SetupPresetData(BundleLot shopBundleLot)
	{
		if (_offerBackground != null)
		{
			UnityEngine.Object.Destroy(_offerBackground.gameObject);
		}
		GameObject gameObject = shopBundleLot.Settings.GetBackground();
		_ = gameObject == null;
		gameObject = UnityEngine.Object.Instantiate(gameObject, _backgroundHolder);
		_offerBackground = gameObject.GetComponent<Background>();
		if (!(_offerBackground == null) && shopBundleLot.Settings.IsCharacterSpriteOverridden)
		{
			Sprite girlSprite = shopBundleLot.Settings.GetGirlSprite();
			if (!(girlSprite == null))
			{
				_offerBackground.Setup(girlSprite);
			}
		}
	}

	public override void Purchase()
	{
		_buyButton.interactable = false;
		BundleLot bundleLot = (BundleLot)base.Source;
		if (bundleLot.Price.Currency.IsRealCurrency())
		{
			string nameForPopup = GetNameForPopup(bundleLot);
			string descriptionForPopup = GetDescriptionForPopup(bundleLot);
			CompositeDisposable onBuyStream = _onBuyStream;
			if (onBuyStream != null)
			{
				onBuyStream.Clear();
			}
			IObservable<Unit> onSuccess = _iapPurchaser.OnSuccess;
			IObservable<string> onFailed = _iapPurchaser.OnFailed;
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<string>(Observable.Take<string>(Observable.TakeUntil<string, Unit>(onFailed, onSuccess), 1), (Action<string>)delegate
			{
				SetupButton(bundleLot);
			}), (ICollection<IDisposable>)_onBuyStream);
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Unit>(Observable.Take<Unit>(Observable.TakeUntil<Unit, string>(onSuccess, onFailed), 1), (Action<Unit>)delegate
			{
				InnerPurchase();
			}), (ICollection<IDisposable>)_onBuyStream);
			_iapPurchaser.BuyProduct(bundleLot.PaymentID, bundleLot.MonetizationID, bundleLot.ID, bundleLot.Price.Value.ToString(), nameForPopup, descriptionForPopup, bundleLot.Data.ImageNameKey, bundleLot.Price.Currency.ToString());
		}
		else
		{
			InnerPurchase();
		}
		void InnerPurchase()
		{
			base.Purchase();
			BundleLot bundleLot2 = (BundleLot)base.Source;
			SetupButton(bundleLot2);
			_selloutPointsView.SetLot(bundleLot);
			_selloutPointsView.OnPurchase(base.Source.Content.AnalyticData.SourceType, _contentSelectorGroup.Current);
			_selloutPointsView.SetLot(bundleLot2);
			_currencyAmplitudeAnalytic.SendSpentEvent(bundleLot.Price.Currency, (int)bundleLot.Price.Value, CurrencyAmplitudeAnalytic.SourceType.OfferMinievents, ContentType.Event, bundleLot.Price.CompositeIdentificator);
		}
	}

	private string GetNameForPopup(BundleLot bundleLot)
	{
		if (!PlatformHelper.IsNutakuMonetization() || bundleLot.Price.Currency != CurrencyType.Real)
		{
			return string.Empty;
		}
		return _localizationService.Text(bundleLot.Data.ItemNameKey);
	}

	private string GetDescriptionForPopup(BundleLot bundleLot)
	{
		if (!PlatformHelper.IsNutakuMonetization() || bundleLot.Price.Currency != CurrencyType.Real)
		{
			return string.Empty;
		}
		return _localizationService.Text(bundleLot.Data.ItemDescriptionKey);
	}

	private void OnDestroy()
	{
		CompositeDisposable onBuyStream = _onBuyStream;
		if (onBuyStream != null)
		{
			onBuyStream.Dispose();
		}
	}

	protected virtual void OnDisable()
	{
		CompositeDisposable onBuyStream = _onBuyStream;
		if (onBuyStream != null)
		{
			onBuyStream.Clear();
		}
	}

	protected virtual void OnEnable()
	{
		if (base.Source != null)
		{
			SetupButton(base.Source as BundleLot);
		}
	}
}
