using System;
using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.Constants;
using GreenT.HornyScapes.Content;
using GreenT.UI;
using StripClub;
using StripClub.Model;
using StripClub.Model.Shop;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.BannerSpace;

public class BannerResourcesWindow : Window
{
	private const int NCount = 10;

	private const int StartValue = 1;

	private const string BannerResourcesMaxKey = "banner_rebuy_max";

	[Header("Add")]
	[SerializeField]
	private BundleResourcesButton _addNButton;

	[SerializeField]
	private BundleResourcesButton _addButton;

	[Header("Remove")]
	[SerializeField]
	private BundleResourcesButton _removeNButton;

	[SerializeField]
	private BundleResourcesButton _removeButton;

	[Header("Info")]
	[SerializeField]
	private Image _resourcesIcon;

	[SerializeField]
	private TMP_InputField _resourcesText;

	[Header("View")]
	[SerializeField]
	private CurrencyColorObserver _priceView;

	[SerializeField]
	private Button _priceButton;

	private StripClub.GameSettings.CurrencySettingsDictionary _currencySettings;

	private ICurrencyProcessor _currencyProcessor;

	private ContentAdder _contentAdder;

	private CurrencyContentFactory _currencyContentFactory;

	private NoCurrencyTabOpener _noCurrencyTabOpener;

	private Banner _source;

	private int _maxLimit;

	private int _currentValue;

	private readonly CompositeDisposable _disposables = new CompositeDisposable();

	[Inject]
	public void Initialization(IConstants<int> constants, ICurrencyProcessor currencyProcessor, GameSettings gameSettings, ContentAdder contentAdder, CurrencyContentFactory currencyContentFactory, NoCurrencyTabOpener noCurrencyTabOpener)
	{
		_contentAdder = contentAdder;
		_currencySettings = gameSettings.CurrencySettings;
		_currencyProcessor = currencyProcessor;
		_currencyContentFactory = currencyContentFactory;
		_noCurrencyTabOpener = noCurrencyTabOpener;
		_maxLimit = constants["banner_rebuy_max"];
	}

	public void Set(Banner banner)
	{
		_source = banner;
		_disposables?.Clear();
		ResetCount();
		SetCurrencyIcon();
		SetupAllButtons();
		SubscribeInput();
	}

	public void SetCount(int count)
	{
		if (count >= 1)
		{
			_currentValue = count;
			UpdateText();
		}
	}

	private void SetCurrencyIcon()
	{
		if (_currencySettings.TryGetValue(_source.BuyPrice.Currency, out var currencySettings))
		{
			_resourcesIcon.sprite = currencySettings.Sprite;
		}
	}

	private void SetupAllButtons()
	{
		SetupButton(_addNButton, 10);
		SetupButton(_addButton, 1);
		SetupButton(_removeNButton, -10);
		SetupButton(_removeButton, -1);
	}

	private void SetupButton(BundleResourcesButton button, int value)
	{
		button.Set(value);
		button.OnClick.Subscribe(ChangeCount).AddTo(_disposables);
	}

	private void SubscribeInput()
	{
		_resourcesText.onValueChanged.AsObservable().Subscribe(OnValueChanged).AddTo(_disposables);
		_priceButton.onClick.AsObservable().Subscribe(delegate
		{
			TryBuy();
		}).AddTo(_disposables);
	}

	private void TryBuy()
	{
		GetTargetPrice(out var total, out var currency);
		if (!_currencyProcessor.TrySpent(currency, total))
		{
			_noCurrencyTabOpener.Open(currency);
			return;
		}
		AddCurrencyContent();
		ResetCount();
		Close();
	}

	private void AddCurrencyContent()
	{
		CurrencyLinkedContent content = _currencyContentFactory.Create(_currentValue, _source.BuyPrice.Currency, new LinkedContentAnalyticData(CurrencyAmplitudeAnalytic.SourceType.Bought), _source.BuyPrice.CompositeIdentificator);
		_contentAdder.AddContent(content);
	}

	private void ChangeCount(int delta)
	{
		_currentValue = Math.Clamp(_currentValue + delta, 1, _maxLimit);
		UpdateText();
	}

	private void OnValueChanged(string input)
	{
		if ((int.TryParse(input, out var result) || string.IsNullOrEmpty(input)) && _currentValue != result)
		{
			_currentValue = Math.Clamp(result, 1, _maxLimit);
			UpdateText();
		}
	}

	private void UpdateText()
	{
		GetTargetPrice(out var total, out var currency);
		Price<int> price = new Price<int>(total, currency, _source.RebuyCost.CompositeIdentificator);
		bool num = string.IsNullOrEmpty(_resourcesText.text);
		_resourcesText.text = _currentValue.ToString();
		if (num)
		{
			SetCaretToEnd();
		}
		_priceView.Set(price);
	}

	private void SetCaretToEnd()
	{
		int length = _resourcesText.text.Length;
		_resourcesText.caretPosition = length;
		_resourcesText.selectionAnchorPosition = length;
		_resourcesText.selectionFocusPosition = length;
	}

	private void GetTargetPrice(out int total, out CurrencyType currency)
	{
		total = _source.RebuyCost.Value * _currentValue;
		currency = _source.RebuyCost.Currency;
	}

	private void OnDisable()
	{
		if (_source != null)
		{
			ResetCount();
		}
	}

	private void ResetCount()
	{
		_currentValue = 1;
		UpdateText();
	}
}
