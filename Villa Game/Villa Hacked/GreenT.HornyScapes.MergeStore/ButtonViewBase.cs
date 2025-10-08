using System;
using System.Collections.Generic;
using StripClub.Model;
using StripClub.Model.Data;
using StripClub.Model.Shop;
using StripClub.UI;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.MergeStore;

public abstract class ButtonViewBase<T> : MonoView<T>
{
	[SerializeField]
	protected Button _buyButton;

	[SerializeField]
	protected Image _currencyType;

	[SerializeField]
	protected TMP_Text _price;

	[SerializeField]
	protected Color _normalColor = Color.white;

	[SerializeField]
	protected Color _lackOfResourceColor = new Color(1f, 0.42f, 0.42f);

	private readonly CompositeDisposable _mainStream = new CompositeDisposable();

	private GameSettings _settings;

	private ICurrencyProcessor _currencyProcessor;

	private Cost _cost;

	private NoCurrencyTabOpener _noCurrencyTabOpener;

	[Inject]
	public void Init(GameSettings settings, ICurrencyProcessor currencyProcessor, NoCurrencyTabOpener noCurrencyTabOpener)
	{
		_settings = settings;
		_currencyProcessor = currencyProcessor;
		_noCurrencyTabOpener = noCurrencyTabOpener;
	}

	public override void Set(T source)
	{
		Dispose();
		base.Set(source);
		CreatClearStream();
		CreatBuyRequestStream();
		CreatChangeCurrencyStream();
		SetPrice();
		SetCurrencyIcon();
		_cost = GetPrice();
		SetStartColor();
	}

	private void CreatClearStream()
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<T>(GetOnClear(), (Action<T>)delegate
		{
			Dispose();
		}), (ICollection<IDisposable>)_mainStream);
	}

	private void CreatBuyRequestStream()
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Unit>(UnityEventExtensions.AsObservable((UnityEvent)_buyButton.onClick), (Action<Unit>)delegate
		{
			TryEmit();
		}), (ICollection<IDisposable>)_mainStream);
	}

	private void TryEmit()
	{
		if (_currencyProcessor.IsEnough(_cost))
		{
			EmitBuyRequest();
		}
		else
		{
			_noCurrencyTabOpener.Open(_cost.Currency);
		}
	}

	private void CreatChangeCurrencyStream()
	{
		DisposableExtensions.AddTo<IDisposable>(_currencyProcessor.GetChangeStream(GetCurrencyType(), OnCurrencyChange), (ICollection<IDisposable>)_mainStream);
	}

	protected abstract void SetPrice();

	private void SetCurrencyIcon()
	{
		if (_settings.CurrencySettings.TryGetValue(GetCurrencyType(), out var currencySettings))
		{
			_currencyType.sprite = currencySettings.Sprite;
			_currencyType.gameObject.SetActive(value: true);
		}
		else
		{
			_currencyType.gameObject.SetActive(value: false);
		}
	}

	private void SetStartColor()
	{
		int count = _currencyProcessor.GetCount(GetCurrencyType());
		OnCurrencyChange(count);
	}

	private void OnCurrencyChange(int value)
	{
		_price.color = (_currencyProcessor.IsEnough(GetCurrencyType(), GetTargetPrice()) ? _normalColor : _lackOfResourceColor);
	}

	protected abstract Cost GetPrice();

	protected abstract CurrencyType GetCurrencyType();

	protected abstract int GetTargetPrice();

	protected abstract IObservable<T> GetOnClear();

	protected abstract void EmitBuyRequest();

	private void Dispose()
	{
		_mainStream.Clear();
	}

	private void OnDestroy()
	{
		_mainStream.Dispose();
	}
}
