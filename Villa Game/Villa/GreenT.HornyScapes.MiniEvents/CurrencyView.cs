using System;
using GreenT.Types;
using StripClub.Model;
using StripClub.Model.Shop;
using StripClub.UI;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class CurrencyView : MonoView
{
	[SerializeField]
	private TMP_Text _amount;

	[SerializeField]
	private Image _icon;

	private ICurrencyProcessor _currencyProcessor;

	private IReadOnlyReactiveProperty<int> _currencyTarget;

	private IDisposable _currencyTargetStream;

	private GameSettings _gameSettings;

	private int _id;

	public bool IsOnCurrencyAnimation { get; set; }

	public int CurrentValue => _currencyTarget.Value;

	[Inject]
	private void Init(ICurrencyProcessor currencyProcessor, GameSettings gameSettings)
	{
		_currencyProcessor = currencyProcessor;
		_gameSettings = gameSettings;
	}

	private void OnDestroy()
	{
		_currencyTargetStream?.Dispose();
	}

	public void Setup(CurrencyType currencyType, CompositeIdentificator currencyIdentificator, bool isTrackable = true, int id = 0)
	{
		_id = id;
		_currencyTargetStream?.Dispose();
		_currencyTarget = _currencyProcessor.GetCountReactiveProperty(currencyType, currencyIdentificator);
		if (isTrackable && _currencyTarget != null)
		{
			_currencyTargetStream = _currencyTarget.Subscribe(delegate
			{
				if (!IsOnCurrencyAnimation)
				{
					SetText();
				}
			});
			SetText();
		}
		SetIcon(currencyType, currencyIdentificator);
	}

	public void SetText(int value)
	{
		_amount.text = $"{value}";
	}

	private void SetText()
	{
		_amount.text = $"{_currencyTarget.Value}";
	}

	private void SetIcon(CurrencyType currencyType, CompositeIdentificator currencyIdentificator)
	{
		if (_gameSettings.CurrencySettings.TryGetValue(currencyType, out var currencySettings, (currencyType != CurrencyType.MiniEvent) ? new CompositeIdentificator(_id) : currencyIdentificator))
		{
			_icon.sprite = currencySettings.Sprite;
		}
	}
}
