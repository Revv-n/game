using System;
using System.Collections.Generic;
using GreenT.HornyScapes;
using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.DebugInfo;
using GreenT.HornyScapes.Events.Content;
using GreenT.HornyScapes.Monetization;
using GreenT.HornyScapes.Sellouts.Views;
using GreenT.Localizations;
using GreenT.Types;
using GreenT.Utilities;
using StripClub.Extensions;
using StripClub.Model;
using StripClub.Model.Shop;
using StripClub.Model.Shop.Data;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace StripClub.UI.Shop;

public class SimpleBundleLotView : LotView
{
	[SerializeField]
	private LocalizedTextMeshPro[] bundleName;

	[SerializeField]
	private Image girl;

	[SerializeField]
	private TMP_Text customizableText;

	[SerializeField]
	private TMProTimer timer;

	[SerializeField]
	private PriceButtonView priceView;

	[SerializeField]
	private GameObject availableAmountRootObject;

	[SerializeField]
	private LocalizedTextMeshPro availableAmountText;

	[SerializeField]
	private LotFeaturesView lotFeaturesView;

	[SerializeField]
	private LocalizedTextMeshPro saleValue;

	[SerializeField]
	private LootboxContentView lootboxContentView;

	[SerializeField]
	private StatableComponent[] availabilityStates;

	[SerializeField]
	private Button buyButton;

	[SerializeField]
	private SelloutPointsView _selloutPointsView;

	[SerializeField]
	private DebugInfoContainer _debugInfo;

	private CurrencyAmplitudeAnalytic _currencyAmplitudeAnalytic;

	private Purchaser _purchaser;

	private Func<DateTime> _getTime;

	private TimeHelper _timeHelper;

	private LocalizationService _localizationService;

	private ContentSelectorGroup _contentSelectorGroup;

	private ListBundleViewData bundleData;

	private bool _isPurchasing;

	private readonly CompositeDisposable _onBuyStream = new CompositeDisposable();

	private IDisposable _availableAmountStream;

	private IDisposable _timerStream;

	private static int DaysInTimer => 1;

	[Inject]
	public void Init(Purchaser purchaser, IClock clock, TimeHelper timeHelper, LocalizationService localizationService, CurrencyAmplitudeAnalytic currencyAmplitudeAnalytic, ContentSelectorGroup contentSelectorGroup)
	{
		_purchaser = purchaser;
		_getTime = clock.GetTime;
		_timeHelper = timeHelper;
		_localizationService = localizationService;
		_currencyAmplitudeAnalytic = currencyAmplitudeAnalytic;
		_contentSelectorGroup = contentSelectorGroup;
	}

	public override void Set(Lot lot)
	{
		BundleLot bundleLot = (BundleLot)lot;
		base.Set((Lot)bundleLot);
		SetLotFeatures(bundleLot);
		TrySetSaleValue(bundleLot);
		SubscribeToLotUpdate(bundleLot);
		SubscribeToTimerComplete();
		SubscribeBuyBtn(bundleLot);
		DisplayContent(bundleLot);
		SetupPresetData(bundleLot);
		SetPrice(bundleLot);
		SetNamesInfo(bundleLot);
		UpdateAvailableAmountInfo(bundleLot);
		ResetDailyInfo();
		SetSelloutLot(bundleLot);
		TryDebug(base.Source);
	}

	private void SetLotFeatures(BundleLot shopBundleLot)
	{
		lotFeaturesView.Set(shopBundleLot.Features);
	}

	private void TrySetSaleValue(BundleLot shopBundleLot)
	{
		if (shopBundleLot.Features.SaleValue.HasValue)
		{
			saleValue.SetArguments(Mathf.Abs(shopBundleLot.Features.SaleValue.Value));
		}
	}

	private void SetNamesInfo(BundleLot shopBundleLot)
	{
		LocalizedTextMeshPro[] array = bundleName;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Init(shopBundleLot.NameKey);
		}
	}

	private void SetPrice(BundleLot shopBundleLot)
	{
		priceView.Set(shopBundleLot.Price, shopBundleLot.OldPrice);
	}

	private void SubscribeToLotUpdate(BundleLot shopBundleLot)
	{
		_availableAmountStream?.Dispose();
		_availableAmountStream = ObservableExtensions.Subscribe<BundleLot>(shopBundleLot.OnUpdated, (Action<BundleLot>)UpdateAvailableAmountInfo);
	}

	private void SubscribeToTimerComplete()
	{
		_timerStream?.Dispose();
		_timerStream = ObservableExtensions.Subscribe<GenericTimer>(timer.Timer.OnTimeIsUp, (Action<GenericTimer>)delegate
		{
			ResetDailyInfo();
		});
	}

	private void ResetDailyInfo()
	{
		base.Source.UpdateDailyInfo();
		DateTime dateTime = _getTime();
		TimeSpan timeLeft = dateTime.Date.AddDays(DaysInTimer) - dateTime;
		timer.Init(timeLeft, _timeHelper.UseCombineFormat);
	}

	private void SetSelloutLot(BundleLot bundleLot)
	{
		_selloutPointsView.SetLot(bundleLot);
		_selloutPointsView.CheckSellout();
	}

	private void UpdateAvailableAmountInfo(Lot shopBundleLot)
	{
		availableAmountRootObject.SetActive(shopBundleLot.AvailableCount != 0);
		int num = shopBundleLot.AvailableCount - shopBundleLot.Received;
		availableAmountText.SetArguments(num, shopBundleLot.AvailableCount);
		int stateNumber = ((shopBundleLot.AvailableCount != 0 && num <= 0) ? 1 : 0);
		StatableComponent[] array = availabilityStates;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Set(stateNumber);
		}
	}

	private void SubscribeBuyBtn(BundleLot lot)
	{
		if (lot.Price.Currency.IsRealCurrency() || !base.Source.IsFree)
		{
			buyButton.onClick.RemoveListener(Purchase);
			buyButton.onClick.AddListener(Purchase);
		}
	}

	public override void Purchase()
	{
		BundleLot bundleLot = (BundleLot)base.Source;
		string nameForPopup = GetNameForPopup(bundleLot);
		string descriptionForPopup = GetDescriptionForPopup(bundleLot);
		CurrencyType currency = bundleLot.Price.Currency;
		CompositeDisposable onBuyStream = _onBuyStream;
		if (onBuyStream != null)
		{
			onBuyStream.Clear();
		}
		_isPurchasing = true;
		switch (currency)
		{
		case CurrencyType.MiniEvent:
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<bool>(_purchaser.OnResult, (Action<bool>)delegate(bool result)
			{
				if (result)
				{
					CompositeDisposable onBuyStream2 = _onBuyStream;
					if (onBuyStream2 != null)
					{
						onBuyStream2.Clear();
					}
					_currencyAmplitudeAnalytic.SendSpentEvent(bundleLot.Price.Currency, (int)bundleLot.Price.Value, CurrencyAmplitudeAnalytic.SourceType.BundlesMinievents, ContentType.Event, bundleLot.Price.CompositeIdentificator);
				}
				_isPurchasing = false;
			}), (ICollection<IDisposable>)_onBuyStream);
			break;
		case CurrencyType.Real:
		{
			BundleLot previousShopBundleLot = (BundleLot)base.Source;
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<bool>(_purchaser.OnResult, (Action<bool>)delegate(bool result)
			{
				BundleLot lot = (BundleLot)base.Source;
				if (result && _isPurchasing)
				{
					_selloutPointsView.SetLot(previousShopBundleLot);
					_selloutPointsView.OnPurchase(base.Source.Content.AnalyticData.SourceType, _contentSelectorGroup.Current);
					_selloutPointsView.SetLot(lot);
				}
				_isPurchasing = false;
			}), (ICollection<IDisposable>)_onBuyStream);
			break;
		}
		}
		_purchaser.TryPurchase(bundleLot, bundleLot.PaymentID, nameForPopup, descriptionForPopup, bundleLot.Data.ImageNameKey);
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
		bundleData = shopBundleLot.Settings.GetBundleData();
		if (bundleData == null)
		{
			return;
		}
		background.sprite = bundleData.Background;
		customizableText.colorGradientPreset = bundleData.ColoGradient;
		customizableText.fontSharedMaterial = bundleData.TextMaterialPreset;
		ShaderFinder.UpdateMaterial(customizableText.fontSharedMaterial);
		Sprite girlSprite;
		if (string.IsNullOrEmpty(shopBundleLot.Settings.CharacterKey))
		{
			girlSprite = bundleData.Girl;
		}
		else
		{
			girlSprite = shopBundleLot.Settings.GetGirlSprite();
			if (girlSprite == null)
			{
				girlSprite = bundleData.Girl;
			}
		}
		girl.sprite = girlSprite;
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
		_onBuyStream.Dispose();
		_availableAmountStream.Dispose();
	}
}
