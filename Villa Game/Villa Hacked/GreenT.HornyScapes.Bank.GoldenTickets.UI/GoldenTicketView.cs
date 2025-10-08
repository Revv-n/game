using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Bank.Offer.UI;
using GreenT.HornyScapes.Monetization;
using GreenT.UI;
using StripClub.Model;
using StripClub.Model.Shop;
using StripClub.UI;
using StripClub.UI.Shop;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Bank.GoldenTickets.UI;

public class GoldenTicketView : MonoView<GoldenTicket>
{
	[SerializeField]
	private PriceView priceTextView;

	[SerializeField]
	private Button buyButton;

	[SerializeField]
	private TMProColorStates statableText;

	[SerializeField]
	private LootboxContentView lootboxContentView;

	[SerializeField]
	private OfferTimer timer;

	[SerializeField]
	private TextMeshProUGUI energyCount;

	private CompositeDisposable onBuyStream = new CompositeDisposable();

	private Purchaser purchaser;

	private SignalBus signalBus;

	private ICurrencyProcessor currencyProcessor;

	private IWindowsManager windowsManager;

	[Inject]
	public void Init(Purchaser purchaser, SignalBus signalBus, ICurrencyProcessor currencyProcessor, IWindowsManager windowsManager)
	{
		this.purchaser = purchaser;
		this.signalBus = signalBus;
		this.currencyProcessor = currencyProcessor;
		this.windowsManager = windowsManager;
	}

	public void SetupButton()
	{
		buyButton.onClick.AddListener(TryPurchase);
	}

	public override void Set(GoldenTicket goldenTicket)
	{
		base.Set(goldenTicket);
		DisplayContent(goldenTicket);
		SetupTimer(goldenTicket);
		SetButton(goldenTicket);
	}

	private void SetButton(GoldenTicket goldenTicket)
	{
		priceTextView.Set(goldenTicket.Price);
		buyButton.interactable = true;
		ChangeState();
	}

	private void ChangeState()
	{
		Price<decimal> price = base.Source.Price;
		if (price.Currency == CurrencyType.Hard)
		{
			statableText.Set((!currencyProcessor.IsEnough(price.Currency, (int)price.Value)) ? 1 : 0);
		}
		else if (price.Currency == CurrencyType.Real)
		{
			statableText.Set(0);
		}
	}

	private void DisplayContent(GoldenTicket goldenTicket)
	{
		energyCount.text = goldenTicket.EnergyDrop.Quantity.ToString();
		if (goldenTicket.Content is LootboxLinkedContent content)
		{
			lootboxContentView.Set(content, goldenTicket.EnergyDrop);
		}
	}

	private void SetupTimer(GoldenTicket goldenTicket)
	{
		timer.Set(goldenTicket.DisplayTimeLocker.Timer);
		goldenTicket.LaunchTimers();
	}

	private void TryPurchase()
	{
		buyButton.interactable = false;
		GoldenTicket source = base.Source;
		CompositeDisposable obj = onBuyStream;
		if (obj != null)
		{
			obj.Clear();
		}
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<bool>(purchaser.OnResult, (Action<bool>)OnPurchaseEnded), (ICollection<IDisposable>)onBuyStream);
		purchaser.TryPurchase(source.Lot, source.Lot.PaymentID, source.Lot.NameKey);
	}

	private void OnPurchaseEnded(bool success)
	{
		if (success)
		{
			ViewUpdateSignal viewUpdateSignal = new ViewUpdateSignal(this);
			signalBus.Fire<ViewUpdateSignal>(viewUpdateSignal);
		}
		SetButton(base.Source);
		windowsManager.Get<RestoreEnergyPopup>().Close();
	}

	private void OnDestroy()
	{
		buyButton.onClick.RemoveAllListeners();
		onBuyStream.Dispose();
	}
}
