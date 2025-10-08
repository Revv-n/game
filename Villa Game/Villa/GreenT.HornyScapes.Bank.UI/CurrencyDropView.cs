using System;
using GreenT.Types;
using Merge;
using StripClub.Model;
using StripClub.UI;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Bank.UI;

public class CurrencyDropView : MonoView
{
	public class Manager : ViewManager<CurrencyDropView>
	{
	}

	[SerializeField]
	private Image icon;

	[SerializeField]
	private TMP_Text count;

	[SerializeField]
	private StatableComponent background;

	private GameSettings gameSettings;

	private IDisposable _iconChangeStream;

	public CurrencyType CurrencyType { get; private set; }

	public CompositeIdentificator CompositeIdentificator { get; private set; }

	[Inject]
	private void Init(GameSettings gameSettings)
	{
		this.gameSettings = gameSettings;
	}

	public void Set(CurrencyType currency, int? quantity, CompositeIdentificator compositeIdentificator)
	{
		CurrencyType = currency;
		CompositeIdentificator = compositeIdentificator;
		SetQuantity(quantity);
		if (!gameSettings.CurrencySettings.TryGetValue(currency, out var currencySettings, compositeIdentificator))
		{
			Debug.LogException(new ArgumentOutOfRangeException($"В словаре [{gameSettings.CurrencySettings.GetType()} отсутствует тип {currency}]"));
			return;
		}
		ApplyData(currencySettings.AlternativeSprite, currency);
		_iconChangeStream = gameSettings.CurrencySettings[currency, compositeIdentificator].ObserveEveryValueChanged((CurrencySettings actualSettings) => actualSettings.AlternativeSprite).Subscribe(delegate(Sprite currentValue)
		{
			ApplyData(currentValue, currency);
		});
	}

	private void SetQuantity(int? quantity)
	{
		if (quantity.HasValue)
		{
			count.SetActive(active: true);
			count.text = quantity.Value.ToString();
		}
		else
		{
			count.SetActive(active: false);
			count.text = string.Empty;
		}
	}

	private void ApplyData(Sprite sprite, CurrencyType type)
	{
		icon.sprite = ((sprite == null) ? gameSettings.CurrencyPlaceholder[type, default(CompositeIdentificator)].AlternativeSprite : sprite);
		background?.Set((int)type);
	}

	private void OnDestroy()
	{
		_iconChangeStream?.Dispose();
	}
}
