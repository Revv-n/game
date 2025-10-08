using System;
using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.Bank.UI;
using GreenT.HornyScapes.Constants;
using StripClub.Extensions;
using StripClub.Model;
using StripClub.Model.Shop;
using StripClub.UI;
using StripClub.UI.Shop;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Messenger.UI;

public class ReplyExtenderWindow : PopupWindow
{
	private static readonly string valuesFormat = "{0}/{1}";

	[SerializeField]
	private LocalizedTextMeshPro info;

	[SerializeField]
	private TextMeshProUGUI currentValue;

	[SerializeField]
	private TextMeshProUGUI afterRefillValue;

	[SerializeField]
	private Button extendBtn;

	[SerializeField]
	private PriceWithFreeView extendPriceView;

	[SerializeField]
	private Button refillBtn;

	[SerializeField]
	private PriceWithFreeView refillPriceView;

	[SerializeField]
	private Button closeBtn;

	private Subject<int> onRefill = new Subject<int>();

	private Subject<int> onUpdateReplyExtensionCount = new Subject<int>();

	private StripClub.Model.IPlayerBasics playerBasics;

	private IConstants<Price<int>> priceConstants;

	private IConstants<float> floatConstants;

	private SavableVariable<int> replyExtensionCount;

	private CompositeDisposable disposables = new CompositeDisposable();

	public IObservable<int> OnRefill => onRefill.AsObservable();

	public IObservable<int> OnUpdateReplyExtensionCount => onUpdateReplyExtensionCount.AsObservable();

	[Inject]
	public void Init(StripClub.Model.IPlayerBasics playerBasics, IConstants<Price<int>> priceConstants, IConstants<float> floatConstants, SavableVariable<int> replyExtensionCount)
	{
		this.playerBasics = playerBasics;
		this.priceConstants = priceConstants;
		this.floatConstants = floatConstants;
		this.replyExtensionCount = replyExtensionCount;
	}

	private void Start()
	{
		closeBtn?.onClick.AddListener(Close);
	}

	private void OnEnable()
	{
		RestorableValue<int> replies = playerBasics.Replies;
		Init(replies);
	}

	public void Init(BoundedValue<int> Energy)
	{
		info.SetArguments(1);
		SetupExtendButton();
		SetupRefillButton(Energy);
		currentValue.text = string.Format(valuesFormat, Energy.Value, Energy.Max);
		afterRefillValue.text = string.Format(valuesFormat, Energy.Value + 1, Energy.Max + 1);
	}

	private void SetupExtendButton()
	{
		Price<int> extendPrice;
		ReactiveProperty<int> playersCurrency;
		bool flag = IsReplyExtensionAvailable(out extendPrice, out playersCurrency);
		extendPriceView.Set(extendPrice);
		extendPriceView.SetValueColor(flag ? 1 : 0);
		IObservable<bool> source = (from _ in extendBtn.OnClickAsObservable()
			select IsReplyExtensionAvailable(out extendPrice, out playersCurrency)).Share();
		source.Where((bool _isAvailable) => _isAvailable).Subscribe(delegate
		{
			ExtendReplies();
		}).AddTo(disposables);
		source.Where((bool _isAvailable) => !_isAvailable).Subscribe(delegate
		{
			SendToBank();
		}).AddTo(disposables);
	}

	private void SetupRefillButton(BoundedValue<int> Energy)
	{
		Price<int> refillPrice;
		ReactiveProperty<int> playersCurrency;
		bool flag = IsRefillAvailable(out refillPrice, out playersCurrency);
		refillPriceView.Set(refillPrice);
		refillPriceView.SetValueColor(flag ? 1 : 0);
		IObservable<bool> source = (from _ in refillBtn.OnClickAsObservable()
			select IsRefillAvailable(out refillPrice, out playersCurrency)).Share();
		source.Where((bool _isAvailable) => _isAvailable).Subscribe(delegate
		{
			Refill();
		}).AddTo(disposables);
		source.Where((bool _isAvailable) => !_isAvailable).Subscribe(delegate
		{
			SendToBank();
		}).AddTo(disposables);
		refillBtn.enabled = Energy.Value != Energy.Max;
		refillBtn.gameObject.SetActive(Energy.Value != Energy.Max);
	}

	private Price<int> GetRefillPrice()
	{
		Price<int> price = priceConstants["cost_answer"];
		return new Price<int>(price.Value * (playerBasics.Replies.Max - playerBasics.Replies.Value), price.Currency, price.CompositeIdentificator);
	}

	public Price<int> GetReplyExtensionPrice()
	{
		Price<int> price = priceConstants["cost_maxanswer"];
		int value = replyExtensionCount.Value;
		float num = Mathf.Pow(floatConstants["factor_maxanswer"], value - 1);
		return new Price<int>(Mathf.CeilToInt((float)price.Value * num), price.Currency, price.CompositeIdentificator);
	}

	private void ExtendReplies()
	{
		if (IsReplyExtensionAvailable(out var replyExtensionPrice, out var playersCurrency))
		{
			playersCurrency.Value -= replyExtensionPrice.Value;
			playerBasics.Replies.UpdateBounds(playerBasics.Replies.Max + 1, 0);
			RestorableValue<int> replies = playerBasics.Replies;
			int value = replies.Value + 1;
			replies.Value = value;
			SavableVariable<int> savableVariable = replyExtensionCount;
			value = savableVariable.Value + 1;
			savableVariable.Value = value;
			onUpdateReplyExtensionCount.OnNext(replyExtensionCount.Value);
			Close();
		}
	}

	private void SendToBank()
	{
		Close();
		windowsManager.Get<BankWindow>().Open();
	}

	private void Refill()
	{
		if (IsRefillAvailable(out var refillPrice, out var playersCurrency))
		{
			playersCurrency.Value -= refillPrice.Value;
			onRefill.OnNext(refillPrice.Value);
			playerBasics.Replies.Add(playerBasics.Replies.Max);
			Close();
		}
	}

	private bool IsRefillAvailable(out Price<int> refillPrice, out ReactiveProperty<int> playersCurrency)
	{
		refillPrice = GetRefillPrice();
		playersCurrency = playerBasics.GetCurrency(refillPrice.Currency);
		return playersCurrency.Value >= refillPrice.Value;
	}

	private bool IsReplyExtensionAvailable(out Price<int> replyExtensionPrice, out ReactiveProperty<int> playersCurrency)
	{
		replyExtensionPrice = GetReplyExtensionPrice();
		playersCurrency = playerBasics.GetCurrency(replyExtensionPrice.Currency);
		return playersCurrency.Value >= replyExtensionPrice.Value;
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		disposables.Clear();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		onUpdateReplyExtensionCount.Dispose();
	}
}
