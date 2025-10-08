using System;
using GreenT.HornyScapes.Bank;
using GreenT.HornyScapes.Saves;
using GreenT.HornyScapes.ToolTips;
using GreenT.Localizations;
using Merge;
using StripClub.Extensions;
using StripClub.Model;
using StripClub.Model.Data;
using StripClub.Model.Shop;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace StripClub.UI.Shop;

public class SummonLotView : LotView
{
	public new class Manager : ViewManager<LotContainer, ContainerView>
	{
		public LotView Display(Lot source)
		{
			return ((IViewManager<SummonLot, LotView>)this).Display((SummonLot)source);
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

	[SerializeField]
	private TextMeshProUGUI[] titleTextFields;

	[SerializeField]
	private TextMeshProUGUI[] descriptionTextFields;

	[SerializeField]
	private TMProTimer timer;

	[SerializeField]
	private GameObject timerContainer;

	[Space]
	[SerializeField]
	private PriceWithFreeView singlePrice;

	[SerializeField]
	private Button singlePurchaseButton;

	[SerializeField]
	private PriceWithFreeView wholesalePrice;

	[SerializeField]
	private Button wholesalePurchaseButton;

	[Space]
	[SerializeField]
	private GameObject TopPurchaseContainer;

	[SerializeField]
	private GameObject MiddlePurchaseContainer;

	[SerializeField]
	private GameObject BottomPurchaseContainer;

	[Space]
	[SerializeField]
	private DropChancesToolTipOpener tooltipOpener;

	public ReactiveProperty<SummonLot> OnSet = new ReactiveProperty<SummonLot>();

	private CompositeDisposable disposables = new CompositeDisposable();

	private ICurrencyProcessor currencyProcessor;

	private BankSectionRedirectDispatcher sectionDispatcher;

	private TimeHelper timeHelper;

	private LocalizationService _localizationService;

	private CompositeDisposable _localizationDisposables = new CompositeDisposable();

	private PreloadContentService _preloadContentService;

	[Inject]
	public void Init(ICurrencyProcessor currencyProcessor, BankSectionRedirectDispatcher sectionDispatcher, TimeHelper timeHelper, LocalizationService localizationService, PreloadContentService preloadContentService)
	{
		this.currencyProcessor = currencyProcessor;
		this.sectionDispatcher = sectionDispatcher;
		this.timeHelper = timeHelper;
		_localizationService = localizationService;
		_preloadContentService = preloadContentService;
	}

	public override void Set(Lot lot)
	{
		disposables.Clear();
		base.Set(lot);
		SummonLot summonLot = (SummonLot)lot;
		PrepareRewards(summonLot);
		if (summonLot.AvailableCount > 0 && summonLot.AvailableCount <= summonLot.Received)
		{
			Display(display: false);
			return;
		}
		_localizationDisposables.Clear();
		TextMeshProUGUI[] array = titleTextFields;
		foreach (TextMeshProUGUI textField in array)
		{
			_localizationService.ObservableText(summonLot.TitleKey).Subscribe(delegate(string text)
			{
				textField.text = text;
			}).AddTo(_localizationDisposables);
		}
		array = descriptionTextFields;
		foreach (TextMeshProUGUI textField in array)
		{
			_localizationService.ObservableText(summonLot.DescriptionKey).Subscribe(delegate(string text)
			{
				textField.text = text;
			}).AddTo(_localizationDisposables);
		}
		SetTimer(summonLot);
		SetButtons(summonLot);
		SetupOpener(summonLot);
		OnSet.Value = summonLot;
	}

	private void PrepareRewards(SummonLot summonLot)
	{
		if (summonLot.SingleRewardSettings.Reward is LootboxLinkedContent lootboxLinkedContent2)
		{
			Prepare(lootboxLinkedContent2);
		}
		if (summonLot.IsFirstReceived() && summonLot.SingleRewardSettings.FirstPurchaseReward is LootboxLinkedContent lootboxLinkedContent3)
		{
			Prepare(lootboxLinkedContent3);
		}
		if (summonLot.WholesaleRewardSettings.Reward is LootboxLinkedContent lootboxLinkedContent4)
		{
			Prepare(lootboxLinkedContent4);
		}
		if (summonLot.IsFirstReceived() && summonLot.WholesaleRewardSettings.FirstPurchaseReward is LootboxLinkedContent lootboxLinkedContent5)
		{
			Prepare(lootboxLinkedContent5);
		}
		void Prepare(LootboxLinkedContent lootboxLinkedContent)
		{
			if (!lootboxLinkedContent.Lootbox.HasPreopenedContent)
			{
				LinkedContent linkedContents = lootboxLinkedContent.Lootbox.PrepareContent();
				_preloadContentService.PreloadRewards(linkedContents);
			}
		}
	}

	private void SetupOpener(SummonLot summonLot)
	{
		DropChanceToolTipSettings settings = tooltipOpener.Settings;
		settings.KeyText = summonLot.DropChancesTitleKey;
		settings.Chances = summonLot.FakeChances;
	}

	private void OnTimeIsUp(GenericTimer obj)
	{
		if (base.Source != null || base.Source is SummonLot)
		{
			Set(base.Source);
		}
	}

	private void SetTimer(SummonLot summonLot)
	{
		if (!summonLot.GetUnlockTime().HasValue)
		{
			timerContainer.SetActive(value: false);
			summonLot.OnUnlockTimeUpdate().Subscribe(delegate
			{
				SetTimer(summonLot);
			}).AddTo(disposables);
			return;
		}
		TimeSpan timeLeft = (summonLot.Bonus?.LastApplied ?? summonLot.LastPurchase) + summonLot.GetUnlockTime().Value - summonLot.GetTime();
		if (timeLeft.Ticks > 0 && timer != null)
		{
			timer.Init(timeLeft, timeHelper.UseCombineFormat);
		}
		if (timerContainer != null)
		{
			timerContainer.SetActive(timeLeft.Ticks > 0);
		}
		timer.Timer.OnTimeIsUp.Subscribe(OnTimeIsUp).AddTo(disposables);
	}

	private void SetButtons(SummonLot summonLot)
	{
		SetSingleButtonInteractable(summonLot);
		bool isAttentionIconsOn = summonLot.IsFree && !IsFreeTimerReloading();
		SetupButton(singlePrice, summonLot.GetSinglePrice(), singlePurchaseButton, wholesale: false, isAttentionIconsOn);
		if (summonLot.IsX10Disabled)
		{
			DisableX10Button();
			return;
		}
		EnableX10Button();
		SetupButton(wholesalePrice, summonLot.WholesaleRewardSettings.Price, wholesalePurchaseButton, wholesale: true, isAttentionIconsOn);
	}

	private void DisableX10Button()
	{
		wholesalePurchaseButton.SetActive(active: false);
		singlePurchaseButton.gameObject.transform.position = MiddlePurchaseContainer.gameObject.transform.position;
		singlePurchaseButton.gameObject.transform.parent = MiddlePurchaseContainer.gameObject.transform;
	}

	private void EnableX10Button()
	{
		wholesalePurchaseButton.SetActive(active: true);
		singlePurchaseButton.gameObject.transform.position = TopPurchaseContainer.gameObject.transform.position;
		singlePurchaseButton.gameObject.transform.parent = TopPurchaseContainer.gameObject.transform;
		wholesalePurchaseButton.gameObject.transform.position = BottomPurchaseContainer.gameObject.transform.position;
		wholesalePurchaseButton.gameObject.transform.parent = BottomPurchaseContainer.gameObject.transform;
	}

	private void SetSingleButtonInteractable(SummonLot summonLot)
	{
		if (summonLot.IsFreeAlways && IsFreeTimerReloading())
		{
			singlePurchaseButton.interactable = false;
		}
		else
		{
			singlePurchaseButton.interactable = true;
		}
	}

	private bool IsFreeTimerReloading()
	{
		return timer.Timer.IsActive.Value;
	}

	private void SetupButton(PriceWithFreeView view, Price<int> price, Button purchaseButton, bool wholesale, bool isAttentionIconsOn)
	{
		view.Set(price, isAttentionIconsOn);
		Cost cost = new Cost(price.Value, price.Currency);
		(from _ in currencyProcessor.GetCountReactiveProperty(price.Currency)
			select CurrencyProcessor.IsEnough(cost) ? 1 : 0).Subscribe(view.SetValueColor).AddTo(disposables);
		IConnectableObservable<bool> connectableObservable = (from _ in purchaseButton.OnClickAsObservable().WithLatestFrom(currencyProcessor.GetCountReactiveProperty(price.Currency), (Unit _, int _count) => _count)
			select CurrencyProcessor.IsEnough(cost)).Publish();
		connectableObservable.Where((bool _isEnough) => _isEnough).Subscribe(delegate
		{
			SendPurchaseRequest(wholesale);
		}).AddTo(disposables);
		connectableObservable.Where((bool _isEnough) => !_isEnough).Subscribe(delegate
		{
			sectionDispatcher.SelectTab(cost.Currency);
		}).AddTo(disposables);
		connectableObservable.Connect().AddTo(disposables);
	}

	private void SendPurchaseRequest(bool wholesale)
	{
		SummonLot summonLot = (SummonLot)base.Source;
		summonLot.Setup(wholesale);
		Purchase();
		if (summonLot.IsFree && summonLot.Bonus != null)
		{
			summonLot.Bonus.OnApplied.OnNext(default(Unit));
			summonLot.Bonus.LastApplied = summonLot.GetTime();
		}
		Set(base.Source);
	}

	private void OnEnable()
	{
		if (base.Source != null)
		{
			Set((SummonLot)base.Source);
		}
	}

	private void OnDisable()
	{
		disposables.Clear();
		_localizationDisposables.Clear();
	}

	private void OnDestroy()
	{
		disposables.Dispose();
		_localizationDisposables.Dispose();
	}
}
