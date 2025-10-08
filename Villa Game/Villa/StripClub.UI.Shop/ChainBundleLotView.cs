using System;
using DG.Tweening;
using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.Events.Content;
using GreenT.HornyScapes.Monetization;
using GreenT.HornyScapes.Sellouts.Views;
using GreenT.Localizations;
using GreenT.Types;
using Merge;
using StripClub.Model;
using StripClub.Model.Shop;
using StripClub.Model.Shop.Data;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace StripClub.UI.Shop;

public class ChainBundleLotView : LotView
{
	public new class Manager : ViewManager<LotContainer, ContainerView>
	{
		public LotView Display(Lot source)
		{
			return ((IViewManager<BundleLot, LotView>)this).Display((BundleLot)source);
		}
	}

	[SerializeField]
	private PriceButtonView priceView;

	[SerializeField]
	private LootboxContentView lootboxContentView;

	[SerializeField]
	private Button buyButton;

	[SerializeField]
	private StatableComponent lockerAvailabilityState;

	[SerializeField]
	private GameObject toolTip;

	[SerializeField]
	private Image _background;

	[SerializeField]
	private Image _arrow;

	[SerializeField]
	private CanvasGroup _canvasGroup;

	[SerializeField]
	private SelloutPointsView _selloutPointsView;

	private CurrencyAmplitudeAnalytic _currencyAmplitudeAnalytic;

	private Purchaser _purchaser;

	private LocalizationService _localization;

	private ContentSelectorGroup _contentSelectorGroup;

	private IDisposable _lotBoughtTrackStream;

	private bool _isPurchasing;

	private readonly CompositeDisposable _onBuyStream = new CompositeDisposable();

	private const int PREVIOUS_LOT_STEP = 1;

	[Inject]
	public void Init(Purchaser purchaser, LocalizationService localization, CurrencyAmplitudeAnalytic currencyAmplitudeAnalytic, ContentSelectorGroup contentSelectorGroup)
	{
		_purchaser = purchaser;
		_localization = localization;
		_currencyAmplitudeAnalytic = currencyAmplitudeAnalytic;
		_contentSelectorGroup = contentSelectorGroup;
	}

	public override void Set(Lot lot)
	{
		base.Set(lot);
		_canvasGroup.alpha = 1f;
		BundleLot bundleLot = (BundleLot)lot;
		priceView.Set(bundleLot.Price, bundleLot.OldPrice);
		SubscribeBuyBtn(bundleLot);
		DisplayContent(bundleLot);
		int num = bundleLot.AvailableCount - bundleLot.Received;
		SetupAvailabilityState(bundleLot.AvailableCount == 0 || num > 0);
		StartTrackPreviousBundle();
		SetSelloutLot(bundleLot);
	}

	public override void Purchase()
	{
		BundleLot bundleLot = (BundleLot)base.Source;
		string nameForPopup = GetNameForPopup(bundleLot);
		string descriptionForPopup = GetDescriptionForPopup(bundleLot);
		_onBuyStream?.Clear();
		_isPurchasing = true;
		_purchaser.OnResult.Subscribe(delegate(bool result)
		{
			if (result)
			{
				SetupAvailabilityState(isAvailable: false);
				_onBuyStream?.Clear();
				if (bundleLot.Price.Currency == CurrencyType.MiniEvent)
				{
					_currencyAmplitudeAnalytic.SendSpentEvent(bundleLot.Price.Currency, (int)bundleLot.Price.Value, CurrencyAmplitudeAnalytic.SourceType.MiniChainBundles, ContentType.Event, bundleLot.Price.CompositeIdentificator);
				}
				if (_isPurchasing)
				{
					_selloutPointsView.OnPurchase(base.Source.Content.AnalyticData.SourceType, _contentSelectorGroup.Current);
				}
			}
			else
			{
				SetupAvailabilityState(isAvailable: true);
			}
			_isPurchasing = false;
		}).AddTo(_onBuyStream);
		_purchaser.TryPurchase(bundleLot, bundleLot.PaymentID, nameForPopup, descriptionForPopup, bundleLot.Data.ImageNameKey);
	}

	public void SetupAvailabilityState(bool isAvailable)
	{
		if (base.Source.Received < base.Source.AvailableCount)
		{
			lockerAvailabilityState.Set(isAvailable ? 1 : 0);
			toolTip.SetActive(!isAvailable);
			buyButton.enabled = isAvailable;
			return;
		}
		lockerAvailabilityState.Set(1);
		toolTip.SetActive(value: false);
		buyButton.interactable = false;
		_canvasGroup.DOFade(0f, 0.5f).OnComplete(delegate
		{
			base.gameObject.SetActive(value: false);
		});
	}

	public void SetupBackground(Sprite background, Sprite arrow)
	{
		_background.sprite = background;
		_arrow.sprite = arrow;
	}

	public void SetupArrow(bool isLast)
	{
		_arrow.SetActive(!isLast);
	}

	private void StartTrackPreviousBundle()
	{
		_lotBoughtTrackStream?.Dispose();
		_lotBoughtTrackStream = signalBus.GetStream<LotBoughtSignal>().Subscribe(delegate(LotBoughtSignal signal)
		{
			if (signal.Lot.TabID == base.Source.TabID && base.Source.SerialNumber - signal.Lot.SerialNumber == 1 && base.Source.Received < base.Source.AvailableCount)
			{
				SetupAvailabilityState(isAvailable: true);
			}
		});
	}

	private void SetSelloutLot(BundleLot bundleLot)
	{
		_selloutPointsView.SetLot(bundleLot);
		_selloutPointsView.CheckSellout();
	}

	private void SubscribeBuyBtn(BundleLot lot)
	{
		buyButton.onClick.RemoveListener(Purchase);
		buyButton.onClick.AddListener(Purchase);
	}

	private void DisplayContent(BundleLot shopBundleLot)
	{
		if (shopBundleLot.Content is LootboxLinkedContent content)
		{
			lootboxContentView.SetGuarantedRewardsWithViewSettings(content, shopBundleLot.Settings);
		}
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

	private string GetLocalization(string key)
	{
		return _localization.Text(key);
	}

	private void OnDestroy()
	{
		_onBuyStream.Dispose();
		_lotBoughtTrackStream?.Dispose();
	}
}
