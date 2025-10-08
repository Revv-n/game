using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Bank.UI;
using GreenT.HornyScapes.Events.Content;
using GreenT.HornyScapes.Monetization;
using GreenT.HornyScapes.Sellouts.Views;
using GreenT.HornyScapes.Subscription;
using GreenT.Localizations;
using GreenT.UI;
using StripClub.Extensions;
using StripClub.Model.Shop;
using StripClub.UI.Shop.Offer;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace StripClub.UI.Shop;

public class SubscriptionLotView : LotView
{
	public new class Manager : ViewManager<LotContainer, ContainerView>
	{
		public LotView Display(Lot source)
		{
			return ((IViewManager<SubscriptionLot, LotView>)this).Display((SubscriptionLot)source);
		}

		public override void HideAll()
		{
			foreach (ContainerView view in views)
			{
				UnityEngine.Object.Destroy(view.gameObject);
			}
			views.Clear();
		}
	}

	private const int NotBoughtMode = 0;

	private const int ProlongMode = 1;

	private const int GoToMode = 2;

	[SerializeField]
	private StatableComponentGroup _stateSwitchGroup;

	[SerializeField]
	private SubscriptionContentView _lootboxContentView;

	[Header("Buy:")]
	[SerializeField]
	private PriceButtonView _activationPriceTextView;

	[SerializeField]
	private Button _activateButton;

	[SerializeField]
	private SelloutPointsView _activationSelloutPointsView;

	[SerializeField]
	private PriceButtonView _prolongPriceTextView;

	[SerializeField]
	private Button _prolongButton;

	[SerializeField]
	private SelloutPointsView _prolongSelloutPointsView;

	[SerializeField]
	private TMProTimer _timer;

	[Header("Go to:")]
	[SerializeField]
	private Button _goToButton;

	[SerializeField]
	private WindowID _offerWindow;

	[SerializeField]
	private OpenSection _sectionOpener;

	private IDisposable _timerStream;

	private readonly CompositeDisposable _onBuyStream = new CompositeDisposable();

	private Purchaser _purchaser;

	private TimeHelper _timeHelper;

	private Background _offerBackground;

	private IWindowsManager _windowsManager;

	private LocalizationService _localization;

	private SubscriptionStorage _subscriptionStorage;

	private ContentSelectorGroup _contentSelectorGroup;

	private bool _isPurchasing;

	private readonly Subject<Unit> _onGoToClick = new Subject<Unit>();

	public IObservable<Unit> OnGoToClick => (IObservable<Unit>)_onGoToClick;

	[Inject]
	public void Init(Purchaser purchaser, SubscriptionStorage subscriptionStorage, LocalizationService localization, TimeHelper timeHelper, IWindowsManager windowsManager, ContentSelectorGroup contentSelectorGroup)
	{
		_purchaser = purchaser;
		_timeHelper = timeHelper;
		_localization = localization;
		_windowsManager = windowsManager;
		_subscriptionStorage = subscriptionStorage;
		_contentSelectorGroup = contentSelectorGroup;
	}

	public override void Set(Lot lot)
	{
		base.Set(lot);
		if (!(lot is SubscriptionLot subscriptionLot))
		{
			throw new InvalidCastException("Lot (ID:" + lot.ID + ") of type: " + $"{lot.GetType()} can't be casted to " + typeof(SubscriptionLot));
		}
		_stateSwitchGroup.Set(0);
		DisplayContent(subscriptionLot);
		SetupButton(subscriptionLot);
		HandleTimer(subscriptionLot);
		SetSelloutLot(subscriptionLot);
	}

	private void HandleTimer(SubscriptionLot subscriptionLot)
	{
		if (!subscriptionLot.ExtensionID.HasValue)
		{
			_stateSwitchGroup.Set(0);
			return;
		}
		_timerStream?.Dispose();
		SubscriptionModel existingSubscription = _subscriptionStorage.Collection.FirstOrDefault((SubscriptionModel subscription) => subscription.BaseID == subscriptionLot.ExtensionID);
		IObservable<SubscriptionModel> observable = ((existingSubscription == null) ? Observable.SelectMany<SubscriptionModel, SubscriptionModel>(Observable.Where<SubscriptionModel>(_subscriptionStorage.OnNew, (Func<SubscriptionModel, bool>)((SubscriptionModel subscription) => subscription.BaseID == subscriptionLot.ExtensionID)), (Func<SubscriptionModel, IObservable<SubscriptionModel>>)WaitForProlong) : (existingSubscription.Duration.IsActive.Value ? Observable.Return<SubscriptionModel>(existingSubscription) : Observable.Select<Unit, SubscriptionModel>((IObservable<Unit>)existingSubscription.OnActivated, (Func<Unit, SubscriptionModel>)((Unit _) => existingSubscription))));
		_timerStream = ObservableExtensions.Subscribe<GenericTimer>(Observable.Where<GenericTimer>(Observable.Do<GenericTimer>(Observable.Select<SubscriptionModel, GenericTimer>(observable, (Func<SubscriptionModel, GenericTimer>)((SubscriptionModel subscription) => subscription.Duration)), (Action<GenericTimer>)delegate(GenericTimer duration)
		{
			bool flag = duration.TimeLeft.Ticks > 0;
			_stateSwitchGroup.Set(flag ? 1 : 0);
		}), (Func<GenericTimer, bool>)((GenericTimer duration) => duration.TimeLeft.Ticks > 0)), (Action<GenericTimer>)delegate(GenericTimer duration)
		{
			_timer.Init(duration, _timeHelper.UseCombineFormat);
		});
	}

	public void SetGoTo()
	{
		_sectionOpener.Set(base.Source.TabID);
		_stateSwitchGroup.Set(2);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Unit>(UnityUIComponentExtensions.OnClickAsObservable(_goToButton), (Action<Unit>)delegate
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			_onGoToClick?.OnNext(Unit.Default);
			_sectionOpener.Open();
		}), (Component)this);
	}

	private IObservable<SubscriptionModel> WaitForProlong(SubscriptionModel subscriptionModel)
	{
		return Observable.Select<Unit, SubscriptionModel>((IObservable<Unit>)subscriptionModel.OnActivated, (Func<Unit, SubscriptionModel>)((Unit _) => subscriptionModel));
	}

	private void SetupButton(SubscriptionLot subscriptionLot)
	{
		_activationPriceTextView.Set(subscriptionLot.Price, subscriptionLot.OldPrice);
		_activateButton.interactable = subscriptionLot.IsAvailable();
		_prolongPriceTextView.Set(subscriptionLot.Price, subscriptionLot.OldPrice);
		_prolongButton.interactable = subscriptionLot.IsAvailable();
	}

	private void SetSelloutLot(SubscriptionLot bundleLot)
	{
		_activationSelloutPointsView.SetLot(bundleLot);
		_activationSelloutPointsView.CheckSellout();
		_prolongSelloutPointsView.SetLot(bundleLot);
		_prolongSelloutPointsView.CheckSellout();
	}

	private void DisplayContent(SubscriptionLot subscriptionLot)
	{
		_lootboxContentView.HideAll();
		_lootboxContentView.SetBooster(subscriptionLot.BoosterReward);
		_lootboxContentView.SetImmediate(subscriptionLot.ImmediateReward);
		_lootboxContentView.SetRecharge(subscriptionLot.RechargeReward);
	}

	public override void Purchase()
	{
		_prolongButton.interactable = false;
		_activateButton.interactable = false;
		SubscriptionLot subscriptionLot = (SubscriptionLot)base.Source;
		string nameForPopup = GetNameForPopup(subscriptionLot);
		string descriptionForPopup = GetDescriptionForPopup(subscriptionLot);
		CompositeDisposable onBuyStream = _onBuyStream;
		if (onBuyStream != null)
		{
			onBuyStream.Clear();
		}
		_isPurchasing = true;
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<bool>(_purchaser.OnResult, (Action<bool>)OnPurchaseEnded), (ICollection<IDisposable>)_onBuyStream);
		_purchaser.TryPurchase(subscriptionLot, subscriptionLot.PaymentID, nameForPopup, descriptionForPopup, subscriptionLot.Data.ImageNameKey);
	}

	private void OnPurchaseEnded(bool success)
	{
		if (success)
		{
			ViewUpdateSignal viewUpdateSignal = new ViewUpdateSignal(this);
			signalBus.Fire<ViewUpdateSignal>(viewUpdateSignal);
			if (_isPurchasing)
			{
				if (_activationSelloutPointsView.gameObject.activeInHierarchy)
				{
					_activationSelloutPointsView.OnPurchase(base.Source.Content.AnalyticData.SourceType, _contentSelectorGroup.Current);
				}
				else if (_prolongSelloutPointsView.gameObject.activeInHierarchy)
				{
					_prolongSelloutPointsView.OnPurchase(base.Source.Content.AnalyticData.SourceType, _contentSelectorGroup.Current);
				}
			}
		}
		_isPurchasing = false;
		SetupButton((SubscriptionLot)base.Source);
	}

	private string GetDescriptionForPopup(SubscriptionLot subscriptionLot)
	{
		_ = string.Empty;
		return GetLocalization(subscriptionLot.Data.ItemDescriptionKey);
	}

	private string GetNameForPopup(SubscriptionLot subscriptionLot)
	{
		return string.Empty;
	}

	private string GetLocalization(string key)
	{
		return _localization.Text(key);
	}

	private void OnDestroy()
	{
		_onGoToClick.Dispose();
		_onBuyStream.Dispose();
		_timerStream?.Dispose();
	}
}
