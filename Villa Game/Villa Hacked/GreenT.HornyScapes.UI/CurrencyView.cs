using System;
using StripClub.Model;
using StripClub.Model.Shop;
using StripClub.UI;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.UI;

public class CurrencyView : MonoView
{
	[SerializeField]
	protected TMP_Text value;

	[SerializeField]
	protected Button button;

	[SerializeField]
	protected CurrencySpriteAttacher currencySpriteAttacher;

	public float chargeDuration;

	public float delayBeforeCharge;

	[SerializeField]
	protected MonoDisplayStrategy displayStrategy;

	protected IReadOnlyReactiveProperty<int> current;

	protected IDisposable updateValueStream;

	protected ICurrencyProcessor currencyProcessor;

	[field: SerializeField]
	public CurrencyType Currency { get; private set; }

	[field: SerializeField]
	public Image Icon { get; private set; }

	[Inject]
	public void Init(ICurrencyProcessor currencyProcessor)
	{
		this.currencyProcessor = currencyProcessor;
	}

	public void SetTransferButtonAvailable(bool interactable)
	{
		if ((bool)button)
		{
			button.interactable = interactable;
		}
	}

	protected virtual void OnEnable()
	{
		if (!SetupCurrency())
		{
			SetupView();
		}
	}

	protected virtual void SetupView()
	{
		TryGetReactiveCount();
		currencySpriteAttacher.SetView();
		value.text = current.Value.ToString();
		updateValueStream?.Dispose();
		updateValueStream = ObservableExtensions.Subscribe<int>(Observable.SelectMany<Pair<int>, int>(Observable.Pairwise<int>((IObservable<int>)current), (Func<Pair<int>, IObservable<int>>)((Pair<int> values) => SmoothAdd(values.Previous, values.Current))), (Action<int>)delegate(int _newValue)
		{
			value.text = _newValue.ToString();
		}, (Action<Exception>)delegate
		{
			Set(current.Value);
		}, (Action)delegate
		{
			Set(current.Value);
		});
	}

	protected void TryGetReactiveCount()
	{
		if (current == null && currencyProcessor.TryGetCountReactiveProperty(Currency, out var count))
		{
			current = count;
		}
	}

	private bool SetupCurrency()
	{
		if (!currencyProcessor.TryGetCountReactiveProperty(Currency, out var count))
		{
			return true;
		}
		current = count;
		return false;
	}

	protected void Set(int value)
	{
		this.value.text = value.ToString();
	}

	protected virtual void OnDisable()
	{
		updateValueStream?.Dispose();
	}

	protected IObservable<int> SmoothAdd(int oldValue, int newValue)
	{
		float periods = chargeDuration / Time.deltaTime;
		periods = Math.Min(Math.Abs(newValue - oldValue), periods);
		return Observable.Select<long, int>(Observable.TakeWhile<long>(Observable.Delay<long>(Observable.EveryUpdate(), TimeSpan.FromSeconds(delayBeforeCharge)), (Func<long, bool>)((long period) => (float)period < periods && newValue == current.Value)), (Func<long, int>)((long period) => (int)Mathf.Lerp(oldValue, newValue, (float)(period + 1) / periods)));
	}

	public override void Display(bool display)
	{
		displayStrategy.Display(display);
	}
}
